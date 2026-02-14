using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IJudgeContext
    {
        public ReactiveProperty<JudgeResult> JudgeResult { get; }
    }
}
