using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Notes;

namespace OctaNotes.Scripts.Play.Model.Judgment.Strategies
{
    /// <summary>
    /// チェインノーツの判定戦略
    /// </summary>
    public class ChainJudgmentStrategy : IJudgmentStrategy
    {
        private readonly TimingWindow _timingWindow;
        private bool _hasDetectedHold;

        public ChainJudgmentStrategy(TimingWindow timingWindow)
        {
            _timingWindow = timingWindow;
            _hasDetectedHold = false;
        }

        public JudgmentEvent? EvaluateButtonPress(Note note, ButtonInputEvent inputEvent)
        {
            // チェインノーツはボタンを押している状態で判定される
            if (inputEvent.IsPressed)
            {
                float timeDiff = inputEvent.Timestamp - note.CorrectTime;
                
                if (_timingWindow.IsWithinWindow(timeDiff))
                {
                    _hasDetectedHold = true;
                    var result = _timingWindow.EvaluateTiming(timeDiff);
                    
                    return new JudgmentEvent(
                        result,
                        inputEvent.Timestamp,
                        inputEvent.Timestamp,
                        note.LanePosition,
                        JudgmentType.Chain
                    );
                }
            }

            return null;
        }

        public JudgmentEvent? EvaluateTimeProgress(Note note, float currentTime)
        {
            float timeDiff = currentTime - note.CorrectTime;
            
            // ミス判定: ウィンドウを過ぎた場合
            if (timeDiff > _timingWindow.GetMissThreshold() && !_hasDetectedHold)
            {
                return new JudgmentEvent(
                    JudgmentResult.Miss,
                    currentTime,
                    currentTime,
                    note.LanePosition,
                    JudgmentType.Chain
                );
            }

            return null;
        }

        public bool RequiresHoldState()
        {
            return true;
        }

        /// <summary>
        /// ホールド状態をチェックする
        /// </summary>
        public bool CheckHoldState(bool isHeld, float timestamp)
        {
            if (isHeld)
            {
                _hasDetectedHold = true;
            }
            return _hasDetectedHold;
        }
    }
}
