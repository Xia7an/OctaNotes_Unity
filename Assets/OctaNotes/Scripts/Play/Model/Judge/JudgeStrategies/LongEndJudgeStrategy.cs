using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class LongEndJudgeStrategy : IJudgeStrategy
    {
        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate)
        {
            JudgeResult result = new JudgeResult()
            {
                effectInvokeTiming = note.justTiming,
                guid = note.guid,
                judge = Judge.NotJudged,
                laneNumber = note.laneNumber,
                timingDiff = TimingDiff.Just
            };
            if (note.timingDelta < 0) return result;
            result.judge = longPushedRate switch
            {
                _ when longPushedRate >= 0.7 => Judge.Perfect,
                _ when longPushedRate >= 0.6 => Judge.Good,
                _ when longPushedRate >= 0.5 => Judge.Bad,
                _ => Judge.Miss
            };
            result.timingDiff = result.judge == Judge.Perfect ? TimingDiff.Just : TimingDiff.Fast;
            return result;
        }
    }
}
