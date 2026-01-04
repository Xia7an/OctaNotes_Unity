namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// 判定イベントを表すValue Object
    /// </summary>
    public readonly struct JudgmentEvent
    {
        public readonly JudgmentResult Result;
        public readonly float EvaluatedTime;
        public readonly float EffectTime;
        public readonly int NotePosition;
        public readonly JudgmentType JudgmentType;

        public JudgmentEvent(
            JudgmentResult result,
            float evaluatedTime,
            float effectTime,
            int notePosition,
            JudgmentType judgmentType)
        {
            Result = result;
            EvaluatedTime = evaluatedTime;
            EffectTime = effectTime;
            NotePosition = notePosition;
            JudgmentType = judgmentType;
        }
    }
}
