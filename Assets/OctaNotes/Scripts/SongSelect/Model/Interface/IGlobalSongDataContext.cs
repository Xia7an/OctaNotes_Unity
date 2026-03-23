using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    // SongSelectシーンで選曲された譜面情報をProjectContextで保持するためのクラス
    public interface IGlobalSongDataContext
    {
        ChartData ChartData { get; set; }
        
        string MusicPath { get; set; }
    }
}
