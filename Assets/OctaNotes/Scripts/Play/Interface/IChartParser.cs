using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IChartParser
    {
        public void LoadChart(string path);
        public Dictionary<double, List<string>> GraphicalChartData { get; }
        public List<List<NoteTiming>> LaneWiseChartData { get; }
        public List<(double,double)> HsChangeData { get; }
    }
}
