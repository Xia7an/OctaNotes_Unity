using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// Processorへの入力コンテキスト（イミュータブル）
    /// </summary>
    public readonly struct NoteProcessorContext
    {
        public readonly LaneJudgmentState State;
        public readonly float CurrentTime;
        public readonly float NoteTime;
        public readonly float TimingWindow;
        public readonly bool IsButtonPressed;
        public readonly NoteType NoteType;
        public readonly int LaneIndex;

        public NoteProcessorContext(
            LaneJudgmentState state,
            float currentTime,
            float noteTime,
            float timingWindow,
            bool isButtonPressed,
            NoteType noteType,
            int laneIndex)
        {
            State = state;
            CurrentTime = currentTime;
            NoteTime = noteTime;
            TimingWindow = timingWindow;
            IsButtonPressed = isButtonPressed;
            NoteType = noteType;
            LaneIndex = laneIndex;
        }

        public float TimeDiff => CurrentTime - NoteTime;
    }
}
