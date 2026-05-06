using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Settings;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class UIStateStore : IUIState, IDispachable
    {
        private readonly IReducer _reducer;
        private readonly PlaySettingsSO _playSettings;
        
        public ReactiveProperty<UIState> State { get; } = new(
            new()
            {
                controlTarget = Target.SongList,
                cursor = new CursorState
                {
                    songIndex = 0,
                    optionIndex = (int)Options.NoteSpeed
                },
                songDataList = new(),
                selectedDifficulty = Difficulty.Dual,
                selectedSongSort = new SongSort { order = SortOrder.Ascending, sortKey = SortKey.Abc },
                noteSpeed = 5.0,
                judgeOffsetMs = 0
            }
        );

        public UIStateStore(IReducer reducer, PlaySettingsSO playSettings)
        {
            _reducer = reducer;
            _playSettings = playSettings;

            State.Value = State.Value with
            {
                noteSpeed = _playSettings.noteSpeed
            };
        }
        
        public void Dispatch(UIAction action)
        {
            var ret =_reducer.Reduce(State.Value, action);
            State.Value = ret;
            
            // ScriptableObjectに変更を反映
            _playSettings.noteSpeed = ret.noteSpeed;
            _playSettings.judgeOffsetMs = ret.judgeOffsetMs;
        }
    }
}
