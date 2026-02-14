using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ILaneOutputPort
    {
        ReactiveProperty<JudgeResult>  JudgeResult { get; }
    }
}
