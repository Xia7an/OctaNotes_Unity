using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    public interface IReducer
    {
        UIState Reduce(UIState oldState, UIAction action);
    }
}
