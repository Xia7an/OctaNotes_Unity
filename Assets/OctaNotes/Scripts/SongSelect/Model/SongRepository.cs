using System.Collections.Generic;
using System.IO;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using UnityEngine;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class SongRepository : ISongRepository
    {
        public List<SongData> SortedSongData { get; } = new(new List<SongData>());
        
        private List<SongData> _rawSongData;

        private void LoadSongData()
        {
            var basePath = Application.persistentDataPath +  "/Charts";
            string[] folders = Directory.GetDirectories(basePath);
        }
    }
}
