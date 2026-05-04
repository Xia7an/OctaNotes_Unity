using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.Play.Model
{
    public class GlobalPlayResultContext :  IGlobalPlayResultContext
    {
        public int Score { get; set; }
        public int MaxCombo { get; set; }
        public ClearMark ClearMark { get; set; }
        public SongData SongData { get; set; }
        
        public Difficulty Difficulty { get; set; }

        public int PerfectCount { get; set; }
        public int GoodCount { get; set; }
        public int BadCount { get; set; }
        public int MissCount { get; set; }
    }
}
