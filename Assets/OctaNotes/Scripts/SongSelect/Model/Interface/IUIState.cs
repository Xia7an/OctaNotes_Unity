using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    public interface IUIState
    {
        ReactiveProperty<UIState> State { get; }
    }
}
