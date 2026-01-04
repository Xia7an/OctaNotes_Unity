using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;

namespace OctaNotes.Scripts.Play.Model
{
    public class LaneHeadIdxState: ILaneIdx
    {
        private List<int> currentNoteIndices = new List<int>();
        
        public LaneHeadIdxState()
        {
            for (int i = 0; i < 8; i++)
            {
                currentNoteIndices.Add(0);
            }
        }
        
        public void Advance(int lane)
        {
            currentNoteIndices[lane]++;
        }

        public int GetHeadIdx(int lane)
        {
            return currentNoteIndices[lane];
        }
    }
}
