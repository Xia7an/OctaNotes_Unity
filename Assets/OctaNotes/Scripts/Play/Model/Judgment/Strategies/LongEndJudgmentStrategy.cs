using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Notes;

namespace OctaNotes.Scripts.Play.Model.Judgment.Strategies
{
    /// <summary>
    /// ロングノーツ終了の判定戦略
    /// </summary>
    public class LongEndJudgmentStrategy : IJudgmentStrategy
    {
        private readonly float _holdStartTime;
        private readonly float _requiredHoldDuration;
        private bool _isHolding;
        private float _holdReleaseTime;

        public LongEndJudgmentStrategy(float holdStartTime, float requiredHoldDuration)
        {
            _holdStartTime = holdStartTime;
            _requiredHoldDuration = requiredHoldDuration;
            _isHolding = false;
            _holdReleaseTime = 0f;
        }

        public JudgmentEvent? EvaluateButtonPress(Note note, ButtonInputEvent inputEvent)
        {
            // ホールド状態の更新
            if (inputEvent.WasPressed)
            {
                _isHolding = true;
            }
            
            if (inputEvent.WasReleased)
            {
                _isHolding = false;
                _holdReleaseTime = inputEvent.Timestamp;
                
                // 終了タイミング近くでリリースされた場合の判定
                float endTime = note.CorrectTime;
                float timeDiff = inputEvent.Timestamp - endTime;
                
                // 早すぎるリリースはミス
                if (timeDiff < -0.15f)
                {
                    return new JudgmentEvent(
                        JudgmentResult.Miss,
                        inputEvent.Timestamp,
                        inputEvent.Timestamp,
                        note.LanePosition,
                        JudgmentType.LongEnd
                    );
                }
            }

            return null;
        }

        public JudgmentEvent? EvaluateTimeProgress(Note note, float currentTime)
        {
            float endTime = note.CorrectTime;
            float timeDiff = currentTime - endTime;
            
            // 終了タイミングを過ぎてもホールドしている場合はPerfect
            if (timeDiff >= 0 && _isHolding)
            {
                return new JudgmentEvent(
                    JudgmentResult.Perfect,
                    currentTime,
                    currentTime,
                    note.LanePosition,
                    JudgmentType.LongEnd
                );
            }
            
            // 終了タイミングを大幅に過ぎた場合はミス
            if (timeDiff > 0.15f && !_isHolding)
            {
                return new JudgmentEvent(
                    JudgmentResult.Miss,
                    currentTime,
                    currentTime,
                    note.LanePosition,
                    JudgmentType.LongEnd
                );
            }

            return null;
        }

        public bool RequiresHoldState()
        {
            return true;
        }

        /// <summary>
        /// ホールド状態を更新する
        /// </summary>
        public void UpdateHoldState(bool isHeld, float timestamp)
        {
            _isHolding = isHeld;
            if (!isHeld)
            {
                _holdReleaseTime = timestamp;
            }
        }
    }
}
