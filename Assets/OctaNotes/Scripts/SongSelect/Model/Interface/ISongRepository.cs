using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    public interface ISongRepository
    {
        List<SongData> SortedSongData { get; }
    }
}
