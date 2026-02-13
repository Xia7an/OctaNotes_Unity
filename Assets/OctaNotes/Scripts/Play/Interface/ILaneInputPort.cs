using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ILaneInputPort
    {
        void UpdateButtonState(ButtonState state);
    }
}
