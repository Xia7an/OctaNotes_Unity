namespace OctaNotes.Scripts.Play.Model.Struct
{
    public struct SongEndState
    {
        public ClearMark clearMark;
    }

    public enum ClearMark
    {
        Ap,
        Fc,
        Clear,
        Failed
    }
}
