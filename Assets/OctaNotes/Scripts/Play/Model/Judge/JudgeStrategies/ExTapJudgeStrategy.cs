using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class ExTapJudgeStrategy : IJudgeStrategy
    {
        private readonly Judge[] pushedJudges = new Judge[]
        {
            Judge.None,   // 早入りBad範囲より前
            Judge.Perfect,    // 早入りBad範囲
            Judge.Perfect,   // 早入りGood範囲
            Judge.Perfect,// Perfect範囲
            Judge.Perfect,   // 遅入りGood範囲
            Judge.Perfect,    // 遅入りBad範囲
            Judge.Miss    // 遅入りBad範囲より後
        };

        private readonly Judge[] notPushedJudges = new Judge[]
        {
            Judge.NotJudged,// 早入りBad範囲より前
            Judge.NotJudged,// 早入りBad範囲
            Judge.NotJudged,// 早入りGood範囲
            Judge.NotJudged,// Perfect範囲
            Judge.NotJudged,// 遅入りGood範囲
            Judge.NotJudged,// 遅入りBad範囲
            Judge.Miss      // 遅入りBad範囲より後
        };
        
        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate)
        {
            bool isPressed = buttonStates[note.laneNumber] == ButtonState.BeginPush;

            float delta = note.timingDelta;
            
            JudgeResult result = new JudgeResult
            {
                judge = Judge.NotJudged,
                timingDiff = TimingDiff.Just,
                laneNumber = note.laneNumber,
                guid = note.guid,
                effectInvokeTiming = note.justTiming + note.timingDelta, // 現在時刻にエフェクトを表示
                isEx = true
            };
            int range = RangeSelect(note.timingDelta);
            if (isPressed)
            {
                result.judge = pushedJudges[range];
                result.timingDiff = range switch
                {
                    >= 0 and <= 2 => TimingDiff.Fast,
                    3 => TimingDiff.Just,
                    _ => TimingDiff.Late
                };
            }
            else
            {
                result.judge = notPushedJudges[range];
                result.timingDiff = TimingDiff.Just;
            }
            return result;
        }

        private int RangeSelect(float timeDelta)
        {
            const float BadThreshold = 0.15f;
            const float GoodThreshold = 0.10f;
            const float PerfectThreshold = 0.05f;
            var range = timeDelta switch
            {
                < -BadThreshold => 0,
                >= -BadThreshold and < -GoodThreshold => 1,
                >= -GoodThreshold and < -PerfectThreshold => 2,
                >= -PerfectThreshold and <= PerfectThreshold => 3,
                > PerfectThreshold and <= GoodThreshold => 4,
                > GoodThreshold and <= BadThreshold => 5,
                _ => 6
            };
            return range;
        }
    }
}
