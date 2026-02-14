using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneInputPort : ILaneInputPort
    {
        private readonly ILaneContext _laneContext;
        private readonly ILaneInputStateWriter _laneInputStateWriter;

        public LaneInputPort(ILaneContext laneContext, ILaneInputStateWriter laneInputStateWriter)
        {
            _laneContext = laneContext;
            _laneInputStateWriter = laneInputStateWriter;
        }

        public void UpdateButtonState(ButtonState state)
        {
            _laneInputStateWriter.SetLaneButtonState(_laneContext.LaneIndex, state);
        }
    }
}
