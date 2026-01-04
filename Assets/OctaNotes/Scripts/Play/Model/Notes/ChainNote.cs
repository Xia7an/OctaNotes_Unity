using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Judgment.Strategies;

namespace OctaNotes.Scripts.Play.Model.Notes
{
    /// <summary>
    /// チェインノーツ
    /// </summary>
    public class ChainNote : Note
    {
        public ChainNote(float correctTime, int lanePosition) : base(correctTime, lanePosition)
        {
        }

        public override JudgmentType GetJudgmentType()
        {
            return JudgmentType.Chain;
        }

        public override IJudgmentStrategy CreateJudgmentStrategy(TimingWindow timingWindow)
        {
            return new ChainJudgmentStrategy(timingWindow);
        }
    }
}
