using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class TapJudgeStrategy : IJudgeStrategy
    {
        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate)
        {
            const float BadThreshold = 0.15f;
            const float GoodThreshold = 0.10f;
            const float PerfectThreshold = 0.05f;

            bool isPressed = buttonStates != null
                             && note.laneNumber >= 0
                             && note.laneNumber < buttonStates.Count
                             && buttonStates[note.laneNumber] == ButtonState.BeginPush;

            float delta = note.timingDelta;
            float absDelta = Math.Abs(delta);

            JudgeResult result = new JudgeResult
            {
                judge = Judge.NotJudged,
                timingDiff = TimingDiff.Just,
                laneNumber = note.laneNumber,
                guid = note.guid,
                effectInvokeTiming = note.justTiming + note.timingDelta // 現在時刻にエフェクトを表示
            };

            if (delta > BadThreshold)
            {
                result.judge = Judge.Miss;
                return result;
            }

            if (!isPressed)
            {
                if (delta < -BadThreshold)
                {
                    result.judge = Judge.NotJudged;
                    return result;
                }

                result.judge = Judge.NotJudged;
                return result;
            }

            if (delta < -BadThreshold)
            {
                result.judge = Judge.None;
                return result;
            }

            if (absDelta > GoodThreshold && absDelta <= BadThreshold)
            {
                result.judge = Judge.Bad;
                result.timingDiff = delta < 0 ? TimingDiff.Fast : TimingDiff.Late;
                return result;
            }

            if (absDelta > PerfectThreshold && absDelta <= GoodThreshold)
            {
                result.judge = Judge.Good;
                result.timingDiff = delta < 0 ? TimingDiff.Fast : TimingDiff.Late;
                return result;
            }

            if (absDelta <= PerfectThreshold)
            {
                result.judge = Judge.Perfect;
                result.timingDiff = TimingDiff.Just;
                return result;
            }

            return result;
        }
    }
}
