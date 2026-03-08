using System;
using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    public interface ISongRepository
    {
        Dictionary<Guid,SongData> SongDataDict { get; }
        
        List<Guid> SongIds { get; }
    }
}
