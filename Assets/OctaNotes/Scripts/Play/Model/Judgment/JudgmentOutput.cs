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
    /// 保留中の判定情報（演出時刻まで発火を遅延）
    /// LongEnd・Chain共通で使用
    /// </summary>
    public readonly struct PendingJudgment
    {
        public readonly JudgmentResult Result;
        public readonly JudgmentType Type;
        public readonly float EvaluatedTime;
        public readonly float EffectTime;
        public readonly int LaneIndex;

        public PendingJudgment(
            JudgmentResult result,
            JudgmentType type,
            float evaluatedTime,
            float effectTime,
            int laneIndex)
        {
            Result = result;
            Type = type;
            EvaluatedTime = evaluatedTime;
            EffectTime = effectTime;
            LaneIndex = laneIndex;
        }

        /// <summary>
        /// 指定時刻が演出時刻に達しているか
        /// </summary>
        public bool IsReadyToFire(float currentTime)
        {
            return currentTime >= EffectTime;
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
        /// <summary>
        /// trueの場合、イベント発火をEffectTimeまで遅延する
        /// </summary>
        public readonly bool IsDelayed;

        public JudgmentOutput(
            JudgmentResult result,
            JudgmentType type,
            float evaluatedTime,
            float effectTime,
            bool shouldAdvanceNote = true,
            StateUpdate? stateUpdate = null,
            bool isDelayed = false)
        {
            Result = result;
            Type = type;
            EvaluatedTime = evaluatedTime;
            EffectTime = effectTime;
            ShouldAdvanceNote = shouldAdvanceNote;
            StateUpdate = stateUpdate;
            IsDelayed = isDelayed;
        }

        /// <summary>
        /// PendingJudgmentに変換する
        /// </summary>
        public PendingJudgment ToPendingJudgment(int laneIndex)
        {
            return new PendingJudgment(Result, Type, EvaluatedTime, EffectTime, laneIndex);
        }
    }
}
