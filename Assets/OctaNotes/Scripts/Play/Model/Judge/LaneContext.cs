namespace OctaNotes.Scripts.Play.Model
{
    public class LaneContext
    {
        public int LaneIndex { get; private set; }

        public LaneContext(int laneIndex)
        {
            LaneIndex = laneIndex;
        }
    }
}
