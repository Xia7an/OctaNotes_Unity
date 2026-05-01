namespace OctaNotes.Scripts.Play.Model.Struct
{
    public struct SongEndState
    {
        ClearMark clearMark;
    }

    public enum ClearMark
    {
        Ap,
        Fc,
        Clear,
        Failed
    }
}
