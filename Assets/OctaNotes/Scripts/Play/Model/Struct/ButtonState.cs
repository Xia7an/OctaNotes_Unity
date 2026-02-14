namespace OctaNotes.Scripts.Play.Model.Struct
{
    public enum ButtonState
    {
        BeginPush, // 押されたフレームのみこの状態になる
        Pushed,    // 押された次のフレームから離されるまでの間はこの状態
        EndPush,   // 離されたフレームのみこの状態になる
        Released   // 離された次のフレームから押されるまでの間はこの状態
    }
}
