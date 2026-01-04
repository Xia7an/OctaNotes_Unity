using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Judgment.Strategies;

namespace OctaNotes.Scripts.Play.Model.Notes
{
    /// <summary>
    /// タップノーツ
    /// </summary>
    public class TapNote : Note
    {
        public TapNote(float correctTime, int lanePosition) : base(correctTime, lanePosition)
        {
        }

        public override JudgmentType GetJudgmentType()
        {
            return JudgmentType.Tap;
        }

        public override IJudgmentStrategy CreateJudgmentStrategy(TimingWindow timingWindow)
        {
            return new TapJudgmentStrategy(timingWindow);
        }
    }
}
