using System;

namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// 状態更新の指示
    /// </summary>
    public readonly struct StateUpdate
    {
        public readonly bool? SetInLongNote;
        public readonly float? SetLongNoteStartTime;
        public readonly bool? SetLongNoteReleased;
        public readonly bool? SetChainNoteJudged;
        public readonly float? SetChainNoteFirstPressTime;

        public StateUpdate(
            bool? setInLongNote = null,
            float? setLongNoteStartTime = null,
            bool? setLongNoteReleased = null,
            bool? setChainNoteJudged = null,
            float? setChainNoteFirstPressTime = null)
        {
            SetInLongNote = setInLongNote;
            SetLongNoteStartTime = setLongNoteStartTime;
            SetLongNoteReleased = setLongNoteReleased;
            SetChainNoteJudged = setChainNoteJudged;
            SetChainNoteFirstPressTime = setChainNoteFirstPressTime;
        }

        /// <summary>
        /// LaneJudgmentStateに状態更新を適用する
        /// </summary>
        public void ApplyTo(LaneJudgmentState state)
        {
            if (SetInLongNote.HasValue)
            {
                if (SetInLongNote.Value && SetLongNoteStartTime.HasValue)
                {
                    state.StartLongNote(SetLongNoteStartTime.Value);
                }
                else if (!SetInLongNote.Value)
                {
                    state.EndLongNote();
                }
            }

            if (SetLongNoteReleased.HasValue)
            {
                state.SetLongNoteReleased(SetLongNoteReleased.Value);
            }

            if (SetChainNoteJudged.HasValue)
            {
                state.SetChainNoteJudged(SetChainNoteJudged.Value);
            }

            if (SetChainNoteFirstPressTime.HasValue)
            {
                state.SetChainNoteFirstPressTime(SetChainNoteFirstPressTime.Value);
            }
        }
    }

    /// <summary>
    /// Processorからの出力（イミュータブル）
    /// </summary>
    public readonly struct JudgmentOutput
    {
        public readonly JudgmentResult Result;
        public readonly JudgmentType Type;
        public readonly float EvaluatedTime;
        public readonly float EffectTime;
        public readonly bool ShouldAdvanceNote;
        public readonly StateUpdate? StateUpdate;

        public JudgmentOutput(
            JudgmentResult result,
            JudgmentType type,
            float evaluatedTime,
            float effectTime,
            bool shouldAdvanceNote = true,
            StateUpdate? stateUpdate = null)
        {
            Result = result;
            Type = type;
            EvaluatedTime = evaluatedTime;
            EffectTime = effectTime;
            ShouldAdvanceNote = shouldAdvanceNote;
            StateUpdate = stateUpdate;
        }
    }
}
