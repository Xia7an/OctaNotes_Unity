using System;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public struct SongData
    {
        public string songName;
        public string musicPath; // Application.PersistentDatapath から音源ファイルへの相対パス
        public string jacketPath;
        public ChartData[]  chartDatas; // [0]: dual, [1]: quad, [2]: octa, [3]: duet?
        public Guid guid;
    }
}
