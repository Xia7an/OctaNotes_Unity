using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class ChartRepository: IChartRepositoryImmutable
    {
        private readonly IChartParser _chartParser;

        public ChartRepository(IChartParser chartParser)
        {
            _chartParser = chartParser;
        }
        
        // 時刻をキーとして、その時刻にはどのレーンにノーツがあるかがわかる辞書
        public Dictionary<double, List<GraphicalNoteEntry>> GraphicalChartData => _chartParser.GraphicalChartData;
        
        // レーン毎のノーツ一覧がリストになって記録される
        // 時刻の昇順に並んでいることを保証する
        public List<List<NoteTiming>> LaneWiseChartData => _chartParser.LaneWiseChartData;
        public List<(double,double)> HsChangeData => _chartParser.HsChangeData;
    }
}
