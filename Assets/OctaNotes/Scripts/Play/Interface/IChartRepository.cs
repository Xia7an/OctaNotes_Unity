using System.Collections.Generic;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IChartRepositoryImmutable
    {
        public List<string> ChartData { get; }
    }
}
