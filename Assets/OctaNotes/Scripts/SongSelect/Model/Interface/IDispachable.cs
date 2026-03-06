using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using UnityEngine.InputSystem;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    public interface IDispachable
    {
        void Dispatch(UIAction action);
    }
}
