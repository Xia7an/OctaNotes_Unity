using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IScoreCalcurator
    {
        ReactiveProperty<int> Score { get; }
    }
}
