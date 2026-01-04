using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Notes;

namespace OctaNotes.Scripts.Play.Model.Judgment.Strategies
{
    /// <summary>
    /// タップノーツの判定戦略
    /// </summary>
    public class TapJudgmentStrategy : IJudgmentStrategy
    {
        private readonly TimingWindow _timingWindow;

        public TapJudgmentStrategy(TimingWindow timingWindow)
        {
            _timingWindow = timingWindow;
        }

        public JudgmentEvent? EvaluateButtonPress(Note note, ButtonInputEvent inputEvent)
        {
            if (!inputEvent.WasPressed)
            {
                return null;
            }

            float timeDiff = inputEvent.Timestamp - note.CorrectTime;
            
            if (!_timingWindow.IsWithinWindow(timeDiff))
            {
                return null;
            }

            var result = _timingWindow.EvaluateTiming(timeDiff);
            
            return new JudgmentEvent(
                result,
                inputEvent.Timestamp,
                inputEvent.Timestamp,
                note.LanePosition,
                JudgmentType.Tap
            );
        }

        public JudgmentEvent? EvaluateTimeProgress(Note note, float currentTime)
        {
            float timeDiff = currentTime - note.CorrectTime;
            
            // ミス判定: ウィンドウを過ぎた場合
            if (timeDiff > _timingWindow.GetMissThreshold())
            {
                return new JudgmentEvent(
                    JudgmentResult.Miss,
                    currentTime,
                    currentTime,
                    note.LanePosition,
                    JudgmentType.Tap
                );
            }

            return null;
        }

        public bool RequiresHoldState()
        {
            return false;
        }
    }
}
