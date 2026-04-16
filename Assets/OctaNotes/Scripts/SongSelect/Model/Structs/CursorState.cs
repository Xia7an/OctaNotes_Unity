namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public record CursorState
    {
        public int songIndex { get; init; }
        public int optionIndex { get; init; }
    }
}
