using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using UnityEngine;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // ファイルから楽曲情報を読み込んで、未ソート済みの楽曲リストを提供するクラス
    public class SongRepository : ISongRepository
    {
        public Dictionary<Guid, SongData> SongDataDict { get; }  = new Dictionary<Guid, SongData>();
        public List<Guid> SongIds { get; }  = new List<Guid>();
        
        private void LoadSongData()
        {
            var basePath = Application.persistentDataPath + "/Charts";
            string[] folders = Directory.GetDirectories(basePath);
            foreach(var folder in folders)
            {
                var path = folder + "/metadata.json";
                RawSongData data;
                try
                {
                    var metadata = File.ReadAllText(path);
                    data = JsonSerializer.Deserialize<RawSongData>(metadata);
                }
                catch(FileNotFoundException)
                {
                    Debug.LogWarning($"Path \"{folder}\" does not contain \"metadata.json\". Skipped.");
                    continue;
                }

                SongData songData = new SongData()
                {
                    songName = data.title,
                    composerName = data.composer,
                    musicPath = data.musicPath,
                    jacketPath = data.jacketPath,
                    chartDatas = new ChartData[]
                    {
                        ConvertLevelData(data.dualData, Difficulty.Dual),
                        ConvertLevelData(data.quadData, Difficulty.Quad),
                        ConvertLevelData(data.octaData, Difficulty.Octa),
                    }
                };
                var songId = Guid.NewGuid();
                SongDataDict.Add(songId,songData);
                SongIds.Add(songId);
            }
        }

        private ChartData ConvertLevelData(LevelData levelData, Difficulty difficulty)
        {
            return new ChartData()
            {
                level = levelData.level,
                chartPath = levelData.chartPath,
                difficulty =  difficulty,
            };
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
        [JsonPropertyName("octa")] public LevelData octaData;
        [JsonPropertyName("quad")] public LevelData quadData;
        [JsonPropertyName("dual")]  public LevelData dualData;
    }

    [Serializable]
    public class LevelData
    {
        public int level;
        public string chartPath;
    }
}
