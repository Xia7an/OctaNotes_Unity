using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.ViewModel.Interface
{
    public interface IJudgeSoundViewModel
    {
        ReactiveProperty<JudgeSound> JudgeForSound { get; }
    }
}
