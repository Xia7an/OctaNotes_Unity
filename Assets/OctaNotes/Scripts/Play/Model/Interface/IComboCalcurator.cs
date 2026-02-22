using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IComboCalcurator
    {
        ReactiveProperty<int> Combo { get; }
    }
}
