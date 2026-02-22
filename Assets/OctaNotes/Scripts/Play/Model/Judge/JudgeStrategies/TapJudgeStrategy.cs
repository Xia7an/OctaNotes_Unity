using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class TapJudgeStrategy : IJudgeStrategy
    {
        private readonly PlaySettingsSO _playSettings;

        public TapJudgeStrategy(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
        }
        private readonly Judge[] pushedJudges = new Judge[]
        {
            Judge.None,   // 早入りBad範囲より前
            Judge.Bad,    // 早入りBad範囲
            Judge.Good,   // 早入りGood範囲
            Judge.Perfect,// Perfect範囲
            Judge.Good,   // 遅入りGood範囲
            Judge.Bad,    // 遅入りBad範囲
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
                effectInvokeTiming = note.justTiming + note.timingDelta // 現在時刻にエフェクトを表示
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
            float badThreshold = _playSettings.badRangeMs / 1000f;
            float goodThreshold =  _playSettings.goodRangeMs / 1000f;
            float perfectThreshold =  _playSettings.perfectRangeMs / 1000f;
            return timeDelta switch
            {
                _ when timeDelta < -badThreshold => 0,
                _ when timeDelta < -goodThreshold => 1,
                _ when timeDelta < -perfectThreshold => 2,
                _ when timeDelta <= perfectThreshold => 3,
                _ when timeDelta <= goodThreshold => 4,
                _ when timeDelta <= badThreshold => 5,
                _ => 6
            };
        }
    }
}
