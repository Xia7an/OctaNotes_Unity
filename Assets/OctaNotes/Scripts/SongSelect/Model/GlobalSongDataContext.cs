using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class GlobalSongDataContext: IGlobalSongDataContext
    {
        public ChartData ChartData { get; set; } = new ChartData();
    }
}
