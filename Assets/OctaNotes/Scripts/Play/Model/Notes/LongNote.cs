using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Judgment.Strategies;

namespace OctaNotes.Scripts.Play.Model.Notes
{
    /// <summary>
    /// ロングノーツ
    /// </summary>
    public class LongNote : Note
    {
        public float StartTime { get; }
        public float EndTime { get; }

        private readonly bool _isStartNote;

        public LongNote(float startTime, float endTime, int lanePosition, bool isStartNote) 
            : base(isStartNote ? startTime : endTime, lanePosition)
        {
            StartTime = startTime;
            EndTime = endTime;
            _isStartNote = isStartNote;
        }

        public override JudgmentType GetJudgmentType()
        {
            return _isStartNote ? JudgmentType.LongStart : JudgmentType.LongEnd;
        }

        public override IJudgmentStrategy CreateJudgmentStrategy(TimingWindow timingWindow)
        {
            if (_isStartNote)
            {
                return new LongStartJudgmentStrategy(timingWindow);
            }
            return new LongEndJudgmentStrategy(StartTime, EndTime - StartTime);
        }
    }
}
