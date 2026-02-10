namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// 各レーンの判定状態を管理するクラス
    /// </summary>
    public class LaneJudgmentState
    {
        public int CurrentNoteIndex { get; private set; }
        public bool IsInLongNote { get; private set; }
        public float LongNoteStartTime { get; private set; }
        public bool LongNoteReleased { get; private set; }
        public bool ChainNoteJudged { get; private set; }
        public float ChainNoteFirstPressTime { get; private set; }

        /// <summary>
        /// 保留中の判定（EffectTimeまで発火を遅延）
        /// </summary>
        public PendingJudgment? PendingJudgment { get; private set; }

        public LaneJudgmentState()
        {
            Reset();
        }

        public void Reset()
        {
            CurrentNoteIndex = 0;
            IsInLongNote = false;
            LongNoteStartTime = 0f;
            LongNoteReleased = false;
            ChainNoteJudged = false;
            ChainNoteFirstPressTime = -1f;
            PendingJudgment = null;
        }

        public void AdvanceNote()
        {
            CurrentNoteIndex++;
        }

        public void StartLongNote(float startTime)
        {
            IsInLongNote = true;
            LongNoteStartTime = startTime;
            LongNoteReleased = false;
        }

        public void EndLongNote()
        {
            IsInLongNote = false;
        }

        public void SetLongNoteReleased(bool released)
        {
            LongNoteReleased = released;
        }

        public void SetChainNoteJudged(bool judged)
        {
            ChainNoteJudged = judged;
        }

        public void SetChainNoteFirstPressTime(float time)
        {
            ChainNoteFirstPressTime = time;
        }

        public void ResetChainState()
        {
            ChainNoteJudged = false;
            ChainNoteFirstPressTime = -1f;
        }

        /// <summary>
        /// 保留判定を設定
        /// </summary>
        public void SetPendingJudgment(PendingJudgment pending)
        {
            PendingJudgment = pending;
        }

        /// <summary>
        /// 保留判定をクリア
        /// </summary>
        public void ClearPendingJudgment()
        {
            PendingJudgment = null;
        }

        /// <summary>
        /// 保留判定があるかどうか
        /// </summary>
        public bool HasPendingJudgment => PendingJudgment.HasValue;
    }
}
