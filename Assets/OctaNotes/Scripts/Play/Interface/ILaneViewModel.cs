using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ILaneViewModel
    {
        ReactiveProperty<ButtonState> ButtonState { get; }
        ReactiveProperty<Judge> CurrentJudge { get; }
    }
}
