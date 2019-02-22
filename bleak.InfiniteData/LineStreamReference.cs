using System.Collections.Generic;

namespace bleak.InfiniteData
{

    public class LineStreamReference
    {
        private object synclock = new object();
        public int PageSize { get; set; }
        public int NumberOfRowsInDataFile { get; set; }

        public int? NumberOfRowsInLineReference { get; set; }

        public LineStreamReference(int pageSize)
        {
            PageSize = pageSize;
        }
        public bool IsHeader { get; set; }
        public int? FirstRowIndex { get; set; }
        public int? LastRowIndex { get; set; }
        public int? Page { get; set; }
        public int? ExtendedFirstRowIndex
        {
            get
            {
                if (!FirstRowIndex.HasValue)
                {
                    return null;
                }
                if (FirstRowIndex.Value - PageSize - 1 < 0)
                {
                    return 0;
                }
                return FirstRowIndex.Value - PageSize - 1;
            }
        }
        public int? ExtendedLastRowIndex
        {
            get
            {
                if (!LastRowIndex.HasValue)
                {
                    return null;
                }
                if (LastRowIndex.Value + PageSize > NumberOfRowsInDataFile)
                {
                    return NumberOfRowsInDataFile - 1;
                }
                return LastRowIndex.Value + PageSize;
            }
        }
        public long ExtendedBeginPosition { get; set; }
        public long BeginPosition { get; set; }
        public long EndPosition { get; set; }
        public long ExtendedEndPosition { get; set; }

        private Dictionary<int, int> _relativeRowIndex = null;
        public Dictionary<int, int> RelativeRowIndex
        {
            get
            {
                if (_relativeRowIndex == null)
                {
                    lock (synclock)
                    {
                        if (_relativeRowIndex == null)
                        {
                            _relativeRowIndex = new Dictionary<int, int>();
                            var relativeIndex = 0;
                            if (ExtendedFirstRowIndex.Value == 0)
                            {
                                for (int relativeKey = ExtendedFirstRowIndex.Value; relativeKey <= ExtendedLastRowIndex; relativeKey++)
                                {
                                    _relativeRowIndex.Add(relativeKey, relativeIndex++);
                                }
                            }
                            else
                            {
                                for (int relativeKey = ExtendedFirstRowIndex.Value + 1; relativeKey <= ExtendedLastRowIndex; relativeKey++)
                                {
                                    _relativeRowIndex.Add(relativeKey, relativeIndex++);
                                }
                            }
                        }
                    }
                }
                return _relativeRowIndex;
            }
        }
    }
}