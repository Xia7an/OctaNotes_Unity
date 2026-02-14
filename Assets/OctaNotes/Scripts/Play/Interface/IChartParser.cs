using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IChartParser
    {
        public void LoadChart(string path);
        
        /// <summary>
        /// ノーツ生成用の時刻おきのノーツ情報
        /// 各時刻ごとのListは要素数40で用いる
        /// 0~7は0番から7番レーンに出現するノーツを記録
        /// 8~39は誘導線の系列0番~31番の始点・中継点・終点を記録
        /// </summary>
        public Dictionary<double, List<GraphicalNoteEntry>> GraphicalChartData { get; }
        
        public List<List<NoteTiming>> LaneWiseChartData { get; }
        public List<(double,double)> HsChangeData { get; }
    }
}
