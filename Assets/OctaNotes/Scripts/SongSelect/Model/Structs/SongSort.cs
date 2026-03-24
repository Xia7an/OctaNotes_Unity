namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public struct SongSort
    {
        public SortOrder order;
        public SortKey sortKey;
    }
    
    public enum SortKey
    {
        Abc,
        Level
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}
