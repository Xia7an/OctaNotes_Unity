using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;

namespace OctaNotes.Scripts.Play.Model
{
    public class ChartRepository: IChartRepositoryImmutable
    {
        public List<string> ChartData { get; private set; }
        private void LoadChart(string path)
        {
            // 譜面データは各行を文字列とするリストで受け取る
            var data = System.IO.File.ReadAllLines(path);
            ChartData = new List<string>(data);
        }

    }
}
