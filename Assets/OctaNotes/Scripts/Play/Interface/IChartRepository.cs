using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IChartRepositoryImmutable
    {
        public Dictionary<double, List<GraphicalNoteEntry>> GraphicalChartData { get; }
        public List<List<NoteTiming>> LaneWiseChartData { get; }
        public List<(double,double)> HsChangeData { get; }

        public int GetNoteCount();

    }
}
