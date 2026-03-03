using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [Serializable]
    public class SongData
    {
        public string songName;
        public string composerName;

        public string musicPath; // 楽曲の音源データへの Application.PersistentDataPath からの相対パス
        public string jacketPath; // ジャケット画像への Application.PersistentDataPath からの相対パス
        
        public List<ChartData> chartData; // chartData[0]: DUAL, chartData[1]: QUAD, chartData[2]: OCTA, (chartData[3]: DUET?)
    }
}
