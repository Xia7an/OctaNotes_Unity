namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// ボタン入力イベントを表すValue Object
    /// </summary>
    public readonly struct ButtonInputEvent
    {
        public readonly float Timestamp;
        public readonly bool IsPressed;
        public readonly bool WasPressed;
        public readonly bool WasReleased;

        public ButtonInputEvent(float timestamp, bool isPressed, bool wasPressed, bool wasReleased)
        {
            Timestamp = timestamp;
            IsPressed = isPressed;
            WasPressed = wasPressed;
            WasReleased = wasReleased;
        }
    }
}
