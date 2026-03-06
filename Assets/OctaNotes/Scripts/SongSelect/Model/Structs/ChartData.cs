using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public struct ChartData
    {
        public string chartPath;
        public Difficulty difficulty;
        public float level; // 譜面のレベルは将来的に小数点以下まで設定される可能性を考慮してfloatとしている。
    }
}
