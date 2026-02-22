using R3;

namespace OctaNotes.Scripts.Play.ViewModel.Interface
{
    public interface IPlayUIViewModel
    {
        ReactiveProperty<string> ComboText { get; }
        ReactiveProperty<string> ScoreText { get; }
    }
}
