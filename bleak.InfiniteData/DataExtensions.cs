using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace bleak.InfiniteData
{

    public static class DataExtensions
    {
        public static string ToDelimitedValue(this DataTable table, string fieldSeparator, string lineSeparator = "\r\n", string textQualifier = null, bool withHeaderRow = true)
        {
            var result = new StringBuilder();
            if (withHeaderRow)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(textQualifier))
                    {
                        result.Append(textQualifier);
                    }
                    result.Append(table.Columns[i].ColumnName);
                    if (!string.IsNullOrEmpty(textQualifier))
                    {
                        result.Append(textQualifier);
                    }
                    result.Append(i == table.Columns.Count - 1 ? lineSeparator : fieldSeparator);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (!string.IsNullOrEmpty(textQualifier))
                    {
                        result.Append(textQualifier);
                    }
                    result.Append(row[i].ToString());
                    if (!string.IsNullOrEmpty(textQualifier))
                    {
                        result.Append(textQualifier);
                    }
                    result.Append(i == table.Columns.Count - 1 ? lineSeparator : fieldSeparator);
                }
            }

            return result.ToString();
        }

        #region Chunking
        /// <summary>Splits a collection into chunks of equal size. The last chunk may be smaller than chunkSize, but all chunks, if any, will contain at least one element.</summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException("chunkSize must be greater than zero.", "chunkSize");
            }

            return chunkIterator(source, chunkSize);
        }
        private static IEnumerable<IEnumerable<T>> chunkIterator<T>(IEnumerable<T> source, int chunkSize)
        {
            var list = new List<T>();
            foreach (var elem in source)
            {
                list.Add(elem);
                if (list.Count == chunkSize)
                {
                    yield return list;
                    list = new List<T>();
                }
            }
            if (list.Count > 0)
            {
                yield return list;
            }
        }

        #endregion Chunking

    }
}