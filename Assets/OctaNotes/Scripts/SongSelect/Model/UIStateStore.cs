using DefaultNamespace;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class UIStateStore : IUIState, IDispachable
    {
        private readonly IReducer _reducer;
        
        public ReactiveProperty<UIState> State { get; } = new(
            new()
            {
                controlTarget = Target.SongList,
                selectedSongIndex = 0,
                songDataList = new(),
                selectedDifficulty = Difficulty.Dual,
                selectedSongSort = new SongSort { order = SortOrder.Ascending, sortKey = SortKey.Abc },
                selectedOptions = Options.NoteSpeed,
            }
        );

        public UIStateStore(IReducer reducer)
        {
            _reducer = reducer;
        }
        
        public void Dispatch(UIAction action)
        {
            State.Value = _reducer.Reduce(State.Value, action);
        }
    }
}
