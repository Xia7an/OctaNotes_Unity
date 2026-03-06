using System;
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
            var basePath = Application.persistentDataPath + "/Charts";
            string[] folders = Directory.GetDirectories(basePath);
            foreach(var path in folders)
            {
                var metadata = File.ReadAllText(path);
            }
        }
    }

    [Serializable]
    public class RawSongData
    {
        public string title;
        public string composer;
        public string jacketPath;
        public string musicPath;
        public string musicOffset;
        
    }
}
