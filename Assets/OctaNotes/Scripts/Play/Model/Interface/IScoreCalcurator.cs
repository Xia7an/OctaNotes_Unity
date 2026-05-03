using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IScoreCalcurator
    {
        ReactiveProperty<int> Score { get; }
        
        int perfectCount { get; }
        int goodCount { get; }
        int badCount { get; }
        int missCount { get; }
    }
}
