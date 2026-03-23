using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // ファイルから楽曲情報を読み込んで、未ソート済みの楽曲リストを提供するクラス
    public class SongRepository : ISongRepository, IInitializable
    {
        public Dictionary<Guid, SongData> SongDataDict { get; }  = new Dictionary<Guid, SongData>();
        public List<Guid> SongIds { get; }  = new List<Guid>();

        public void Initialize()
        {
            LoadSongData();
        }
        
        private void LoadSongData()
        {
            var basePath = Application.persistentDataPath + "/Charts";
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string[] folders = Directory.GetDirectories(basePath);
            foreach(var folder in folders)
            {
                if (Path.GetFileName(folder) == ".ignore") continue;
                var path = folder + "/metadata.json";
                RawSongData data;
                try
                {
                    var metadata = File.ReadAllText(path);
                    data = JsonSerializer.Deserialize<RawSongData>(metadata, options);
                }
                catch(FileNotFoundException)
                {
                    Debug.LogWarning($"Path \"{folder}\" does not contain \"metadata.json\". Skipped.");
                    continue;
                }
                catch(JsonException e)
                {
                    Debug.LogWarning($"Path \"{folder}\" contains invalid \"metadata.json\". Skipped. ({e.Message})");
                    continue;
                }

                var chartDataList = new List<ChartData>();
                if (data.dualData != null) chartDataList.Add(ConvertLevelData(data.dualData, Difficulty.Dual, folder));
                if (data.quadData != null) chartDataList.Add(ConvertLevelData(data.quadData, Difficulty.Quad, folder));
                if (data.octaData != null) chartDataList.Add(ConvertLevelData(data.octaData, Difficulty.Octa, folder));

                SongData songData = new SongData()
                {
                    songName = data.title,
                    composerName = data.composer,
                    musicPath = folder + "/" + data.musicPath,
                    jacketPath = folder + "/" + data.jacketPath,
                    chartDatas = chartDataList.ToArray()
                };
                var songId = Guid.NewGuid();
                SongDataDict.Add(songId,songData);
                SongIds.Add(songId);
            }
        }

        private ChartData ConvertLevelData(LevelData levelData, Difficulty difficulty, string folder)
        {
            return new ChartData()
            {
                level = levelData.level,
                chartPath = folder + "/" + levelData.chartPath,
                difficulty =  difficulty,
            };
        }

    }

    [Serializable]
    public class RawSongData
    {
        public string title { get; set; }
        public string composer { get; set; }
        public string jacketPath { get; set; }
        public string musicPath { get; set; }
        public float musicOffset { get; set; }
        [JsonPropertyName("octa")] public LevelData octaData { get; set; }
        [JsonPropertyName("quad")] public LevelData quadData { get; set; }
        [JsonPropertyName("dual")]  public LevelData dualData { get; set; }
    }

    [Serializable]
    public class LevelData
    {
        public int level { get; set; }
        public string chartPath { get; set; }
    }
}
