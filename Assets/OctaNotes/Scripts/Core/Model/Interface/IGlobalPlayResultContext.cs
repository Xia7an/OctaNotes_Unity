using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    // プレイした楽曲の情報とプレイデータが格納される。ProjectContextでDIできる。
    public interface IGlobalPlayResultContext
    {
        int Score { get; set; }
        int MaxCombo { get; set; }
        ClearMark  ClearMark { get; set; }
        SongData SongData { get; set; }
        Difficulty Difficulty { get; set; }
        
        int PerfectCount { get; set; }
        int GoodCount { get; set; }
        int BadCount { get; set; }
        int MissCount { get; set; }
        
    }
}
