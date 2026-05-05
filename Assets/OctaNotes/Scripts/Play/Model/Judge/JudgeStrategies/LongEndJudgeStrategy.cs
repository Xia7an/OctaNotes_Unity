using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class LongEndJudgeStrategy : IJudgeStrategy
    {
        private readonly ILongMiddleHandler _longMiddleHandler;

        public LongEndJudgeStrategy(ILongMiddleHandler longMiddleHandler)
        {
            _longMiddleHandler = longMiddleHandler;
        }

        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate)
        {
            var resolvedLongPushedRate = _longMiddleHandler.TryGetLongEndPushedRate(note.guid, out var cachedRate)
                ? cachedRate
                : 0f;

            JudgeResult result = new JudgeResult()
            {
                effectInvokeTiming = note.justTiming,
                guid = note.guid,
                judge = Judge.NotJudged,
                laneNumber = note.laneNumber,
                timingDiff = TimingDiff.Just
            };
            if (note.timingDelta < 0) return result;
            result.judge = resolvedLongPushedRate switch
            {
                _ when resolvedLongPushedRate >= 0.7 => Judge.Perfect,
                _ when resolvedLongPushedRate >= 0.6 => Judge.Good,
                _ when resolvedLongPushedRate >= 0.5 => Judge.Bad,
                _ => Judge.Miss
            };
            result.timingDiff = result.judge == Judge.Perfect ? TimingDiff.Just : TimingDiff.Fast;
            return result;
        }
    }
}
