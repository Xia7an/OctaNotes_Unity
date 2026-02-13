using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ILaneOutputPort
    {
        Observable<JudgeResult> OnJudgeResult { get; }
    }
}
