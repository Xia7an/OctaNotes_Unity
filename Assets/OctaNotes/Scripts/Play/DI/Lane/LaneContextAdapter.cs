using OctaNotes.Scripts.Play.Model.Interface;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneContextAdapter : ILaneContext
    {
        public int LaneIndex { get; }

        public LaneContextAdapter(int laneIndex)
        {
            LaneIndex = laneIndex;
        }
    }
}

