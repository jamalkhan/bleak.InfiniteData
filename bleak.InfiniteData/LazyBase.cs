using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace bleak.InfiniteData
{

    /// <summary>
    /// LazyDataTable for reading through a CSV / Delimited File using as few resources as possible (RAM, objects, etc.)
    /// </summary>
    public class LazyBase : IEnumerable
    {
        #region Fields
        private FileStream FileStream { get; set; }

        internal object syncRoot = new object();
        #endregion Fields

        #region Properties
        /// <summary>
        /// The referenced File associated with this LazyDataTable
        /// </summary>
        /// <example>C:\temp\file.csv</example>
        public string FileName { get; private set; }

        /// <summary>
        /// The separating delimiter of the File
        /// </summary>
        /// <example>,|\t</example>
        public string Delimiter { get; private set; }

        /// <summary>
        /// Text Qualifer of the file
        /// </summary>
        /// <example>"</example>
        public string TextQualifer { get; private set; }

        /// <summary>
        /// Is First Row a Header Row. 
        /// </summary>
        /// <remarks>True is required</remarks>
        public bool FirstRowHasHeaders { get; private set; }

        /// <summary>
        /// Number of Rows to Load into memory at a time
        /// </summary>
        public int RowsToCache { get; private set; }

        /// <summary>
        /// File Encoding
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// File Stream Reference Data.
        /// </summary>
        public List<LineStreamReference> LineReferences { get; private set; } = new List<LineStreamReference>();

        /// <summary>
        /// Headers in the File
        /// </summary>
        /// <remarks>Should be identical to <seealso cref="Columns"/></remarks>
        public List<string> Headers { get; private set; } = new List<string>();

        /// <summary>
        /// Current Page of in recordset
        /// </summary>
        public int CurrentPage { get; internal set; } = 0;

        public LineStreamReference CurrentLineReference
        {
            get
            {
                return LineReferences[CurrentPage + 1];
            }
        }

        /// <summary>
        /// Most Recently accessed row.
        /// </summary>
        public int FileIndex { get; internal set; } = 0;

        // TODO: Make this configurable?
        public int MaxDegreesOfParallelism { get; private set; } = 50;

        /// <summary>
        /// Gets the Number of Rows in the data file
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Constructor for LazyDataTable
        /// </summary>
        /// <param name="filename">Delimited File of the LazyDataTable</param>
        /// <param name="delimiter">Delimiter of the File</param>
        /// <param name="textqualifer">Text Qualifier of the File</param>
        /// <param name="firstRowHasHeaders">Whether or not First Row Has Headers</param>
        /// <param name="rowsToCache">Number of Rows to access at once</param>
        public LazyBase(string filename, string delimiter, string textqualifer, bool firstRowHasHeaders, int rowsToCache = 10000) : base()
        {
            if (!firstRowHasHeaders)
            {
                throw new ArgumentOutOfRangeException("firstRowHasHeaders", "LazyBase only works against CSVs with headers.");
            }

            FileName = filename;
            Encoding = FileHelper.GetEncoding(FileName);
            FileStream = File.Open(FileName, FileMode.Open, FileAccess.Read);
            Delimiter = delimiter;
            TextQualifer = textqualifer;
            FirstRowHasHeaders = firstRowHasHeaders;
            RowsToCache = rowsToCache;
            LoadHeaders();
            ScanFile();
        }
        #endregion Constructor

        private const int LineEndianLF = 10;
        private const int LineEndianCR = 13;
        private const int StreamEndian = 65535;
        private const int FileEndian = -1;
        protected int filePosition = 0;
        private int[] LineEndians = new int[] { LineEndianCR, LineEndianLF };
        private int[] AllEndians = new int[] { LineEndianCR, LineEndianLF, FileEndian, StreamEndian };
        private void ScanFile()
        {
            int character;
            int page = 0;
            int startRowIndex = 0;
            long beginPosition = 0;
            bool firstRowInSet = true;
            bool hasData = false;
            bool lineHasData = false;
            Count = 0;
            int numberOfRows = 0;
            do
            {
                character = FileStream.ReadByte();
                charMap.Add(filePosition, (char)character);
                filePosition++;

                // If an Line Endian
                if (LineEndians.Contains(character))
                {
                    lineHasData = false;
                    do
                    {
                        character = FileStream.ReadByte();
                        charMap.Add(filePosition, (char)character);
                        filePosition++;

                    } while (character >= 0 && AllEndians.Contains(character));
                    if (!hasData && !AllEndians.Contains(character))
                    {
                        hasData = true;
                    }
                    if (!lineHasData && !AllEndians.Contains(character))
                    {
                        lineHasData = true;
                    }
                }
                if (firstRowInSet)
                {
                    beginPosition = filePosition - 1;
                    firstRowInSet = false;
                }

                // Scan until the end of the row.
                do
                {
                    character = FileStream.ReadByte();
                    charMap.Add(filePosition, (char)character);
                    filePosition++;
                    if (!hasData && !AllEndians.Contains(character))
                    {
                        hasData = true;
                    }
                    if (!lineHasData && !AllEndians.Contains(character))
                    {
                        lineHasData = true;
                    }
                } while (character >= 0 && !AllEndians.Contains(character));

                if (hasData)
                {
                    if (lineHasData)
                    {
                        numberOfRows++;
                        Count++;
                    }
                    if (numberOfRows % RowsToCache == 0)
                    {

                        SaveReference(
                            page: page++,
                            firstRowIndex: startRowIndex,
                            lastRowIndex: Count - 1,
                            numberOfRows: numberOfRows,
                            beginPosition: beginPosition,
                            endPosition: filePosition - 1
                        );
                        startRowIndex = Count;
                        numberOfRows = 0;
                        firstRowInSet = true;
                        hasData = false;
                    }
                }
            } while (character >= 0);

            if (hasData)
            {
                if (numberOfRows > 0)
                {
                    SaveReference(
                        page: page++,
                        firstRowIndex: startRowIndex,
                        lastRowIndex: Count - 1,
                        numberOfRows: numberOfRows,
                        beginPosition: beginPosition,
                        endPosition: filePosition);
                }
            }

            CalculateExtendedFields();
            //CalculateRelativeRowReferences();
        }

        public Dictionary<int, char> charMap = new Dictionary<int, char>();

        //private void CalculateRelativeRowReferences()
        //{
        //    foreach (var lineReference in LineReferences.Where(x => x.FirstRowIndex >= 0))
        //    {
        //        int relativeIndex = 0;
        //        for (int i = lineReference.ExtendedFirstRowIndex.Value; i <= lineReference.ExtendedLastRowIndex; i++)
        //        {
        //            if (lineReference.ExtendedFirstRowIndex > 0)
        //            {
        //                lineReference.RelativeRowIndex.Add(i, relativeIndex);
        //            }
        //            else
        //            {
        //                lineReference.RelativeRowIndex.Add(i, relativeIndex);
        //            }
        //            relativeIndex++;
        //        }
        //    }
        //}

        private void CalculateExtendedFields()
        {
            for (int i = 0; i < LineReferences.Count; i++)
            {
                LineReferences[i].NumberOfRowsInDataFile = Count;
                if (i == 0)
                {
                    LineReferences[i].ExtendedBeginPosition = LineReferences[i].BeginPosition;
                }
                else
                {
                    var extendedBeginPosition = LineReferences[i - 1].BeginPosition;
                    var extendedMinRow = LineReferences[i - 1].FirstRowIndex.HasValue ? LineReferences[i - 1].FirstRowIndex.Value : 0;
                    LineReferences[i].ExtendedBeginPosition = extendedBeginPosition == 0 ? LineReferences[i].BeginPosition : extendedBeginPosition;
                }

                if (i == LineReferences.Count - 1)
                {
                    LineReferences[i].ExtendedEndPosition = LineReferences[i].EndPosition;
                }
                else
                {
                    LineReferences[i].ExtendedEndPosition = LineReferences[i + 1].EndPosition;
                }
            }
        }

        internal string GetHeaderLine()
        {
            int ch;
            List<byte> bytes = new List<byte>();
            string headerline = null;
            var preamble = Encoding.GetPreamble();
            ch = FileStream.ReadByte();
            charMap.Add(filePosition, (char)ch);
            filePosition++;


            do
            {
                if (LineEndians.Contains(ch))
                {
                    //if no BOM 
                    if (preamble.Where((p, i) => p != bytes[i]).Any())
                    {
                        headerline = Encoding.GetString(bytes.ToArray());
                        break;
                    }
                    else
                    //skip BOM
                    {
                        bytes.RemoveRange(0, preamble.Length);
                        headerline = Encoding.GetString(bytes.ToArray());
                        break;
                    }
                }
                bytes.Add(Convert.ToByte(ch));
                ch = FileStream.ReadByte();
                charMap.Add(filePosition, (char)ch);
                filePosition++;
            } while (ch >= 0);

            SaveReference(
                page: null,
                firstRowIndex: null,
                lastRowIndex: null,
                numberOfRows: null,
                beginPosition: 0,
                endPosition: filePosition,
                isHeader: true);
            return headerline.Trim();
        }

        private void SaveReference(
            int? page,
            int? firstRowIndex,
            int? lastRowIndex,
            int? numberOfRows,
            long beginPosition,
            long endPosition,
            bool isHeader = false)
        {
            var reference = new LineStreamReference(RowsToCache)
            {
                IsHeader = isHeader,
                Page = page,
                FirstRowIndex = firstRowIndex,
                LastRowIndex = lastRowIndex,
                NumberOfRowsInLineReference = numberOfRows,
                BeginPosition = beginPosition,
                EndPosition = endPosition
            };
            LineReferences.Add(reference);
        }

        protected virtual void LoadHeaders()
        {
            if (FirstRowHasHeaders)
            {
                Headers.AddRange(GetValues(GetHeaderLine()));
            }
        }

        internal string[] GetValues(string line1)
        {
            return line1.QualifiedSplit(Delimiter, TextQualifer);
        }

        /// <summary>
        /// Releases all resources used by the LazyDataTable
        /// </summary>
        public virtual void Dispose()
        {
            if (FileStream != null)
            {
                FileStream.Dispose();
            }
            GC.Collect();
        }

        internal virtual void ProcessLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                try
                {
                    ProcessLine(line);
                }
                catch (Exception)
                {
                }
            }
        }

        internal virtual void ProcessLine(string line)
        {
            //var vals = GetValues(line);

            ////if (vals.Length > Table.Columns.Count)
            ////{
            ////    continue;
            ////}
            ////var row = Table.NewRow();
            //var columnIndex = 0;
            //foreach (var val in vals)
            //{
            //    //row[columnIndex++] = val.Trim();
            //}
            ////Table.Rows.Add(row);
            throw new NotImplementedException();
        }

        internal virtual void GetBatch()
        {
            if (CurrentLineReference == null)
            {
                throw new IndexOutOfRangeException();
            }

            var lines = new List<string>();

            FileStream.Seek(CurrentLineReference.ExtendedBeginPosition, SeekOrigin.Begin);
            int ch = -1;
            List<byte> bytes = new List<byte>();

            while (FileStream.Position <= CurrentLineReference.ExtendedEndPosition
                &&
                (ch = FileStream.ReadByte()) >= 0)
            {
                if (LineEndians.Contains(ch))
                {
                    var val = Encoding.GetString(bytes.ToArray()).Trim();
                    if (!string.IsNullOrEmpty(val))
                    {
                        lines.Add(val);
                    }
                    bytes = new List<byte>();
                    continue;
                }
                else
                {
                    bytes.Add(Convert.ToByte(ch));
                }
            }
            if (ch == -1)
            {
                var val = Encoding.GetString(bytes.ToArray()).Trim();
                if (!lines.Contains(val) && !string.IsNullOrEmpty(val))
                {
                    lines.Add(val);
                }
            }
            ProcessLines(lines);
        }

        /// <summary>
        /// Returns an Enumerator that iterates through the rows in the File
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<DataRow> GetEnumerator()
        {
            throw new NotImplementedException();
            //for (FileIndex = 0; this.Count - 1 > FileIndex; FileIndex++)
            //{
            //    if (FileIndex == 0)
            //    {
            //        // at first row and no records in table, must be new read.
            //        GetBatch();
            //    }
            //    if (FileIndex >= PageMax)
            //    {
            //        CurrentPage = (int)Math.Floor(((double)FileIndex / (double)RowsToCache));
            //        GetBatch();
            //    }

            //    if (FileIndex + RowsToCache - 1 >= ExtendedPageMax)
            //    {
            //        lock (syncRoot)
            //        {
            //            /*if (Table.Rows.Count > 0)
            //            {
            //                Table.Clear();
            //                GC.Collect();
            //            }*/
            //        }
            //    }

            //    yield return Table.Rows[RelativeIndex];
            //}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}