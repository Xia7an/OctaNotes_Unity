using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class ChartRepository: IChartRepositoryImmutable
    {
        [Inject] private readonly IChartParser _chartParser;
        
        public Dictionary<double, List<string>> GraphicalChartData => _chartParser.GraphicalChartData;
        public List<List<NoteTiming>> LaneWiseChartData => _chartParser.LaneWiseChartData;
        public List<(double,double)> HsChangeData => _chartParser.HsChangeData;
    }
}
