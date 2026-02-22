using OctaNotes.Scripts.Play.Model.Enum;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IJudgeSoundViewModel
    {
        ReactiveProperty<Judge> JudgeForSound { get; }
    }
}
