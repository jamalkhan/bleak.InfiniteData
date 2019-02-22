using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace bleak.InfiniteData
{

    /// <summary>
    /// LazyDataTable for reading through a CSV / Delimited File using as few resources as possible (RAM, objects, etc.)
    /// </summary>
    public class LazyDataTable
        : LazyBase
        , IEnumerable<DataRow>
        , IEnumerable
        , IDisposable
    {
        #region Fields
        public DataTable Table { get; set; } = new DataTable();
        #endregion Fields

        #region Properties

        /// <summary>
        /// Columns in the DataTable
        /// </summary>
        /// /// <remarks>Should be identical to <seealso cref="Headers"/></remarks>
        public IEnumerable<DataColumn> Columns { get { return Table.Columns.Cast<DataColumn>(); } }

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
        public LazyDataTable(string filename, string delimiter, string textqualifer, bool firstRowHasHeaders, int rowsToCache = 10000)
            : base(filename, delimiter, textqualifer, firstRowHasHeaders, rowsToCache)
        {
        }
        #endregion Constructor

        protected override void LoadHeaders()
        {
            if (FirstRowHasHeaders)
            {
                Headers.AddRange(GetValues(GetHeaderLine()));
                foreach (var header in Headers)
                {
                    Table.Columns.Add(header);
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the LazyDataTable
        /// </summary>
        public override void Dispose()
        {
            Table.Clear();
            GC.Collect();
            base.Dispose();
        }


        internal override void GetBatch()
        {
            Table.Clear();
            GC.Collect();
            base.GetBatch();
        }

        internal override void ProcessLine(string line)
        {
            var vals = GetValues(line);

            if (vals.Length > Table.Columns.Count)
            {
                return;
            }
            var row = Table.NewRow();
            var columnIndex = 0;
            foreach (var val in vals)
            {
                row[columnIndex++] = val.Trim();
            }
            Table.Rows.Add(row);
        }

        /// <summary>
        /// Returns an Enumerator that iterates through the rows in the File
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<DataRow> GetEnumerator()
        {
            for (FileIndex = 0; this.Count > FileIndex; FileIndex++)
            {
                if (FileIndex == 0 && Table.Rows.Count == 0)
                {
                    // at first row and no records in table, must be new read.
                    GetBatch();
                }
                else if (FileIndex >= CurrentLineReference.LastRowIndex)
                {
                    CurrentPage = (int)Math.Floor(((double)FileIndex / (double)RowsToCache));
                    GetBatch();
                }

                yield return Table.Rows[CurrentLineReference.RelativeRowIndex[FileIndex]];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}