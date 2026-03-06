using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class UIStateStore : IUIState, IDispachable
    {
        public ReactiveProperty<UIState> State { get; } = new();
        
        public void Dispatch(UIAction action)
        {
            throw new System.NotImplementedException();
        }
    }
}
