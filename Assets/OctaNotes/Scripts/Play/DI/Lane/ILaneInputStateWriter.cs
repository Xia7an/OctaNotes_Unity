using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public interface ILaneInputStateWriter
    {
        void SetLaneButtonState(int laneIndex, ButtonState state);
    }
}
