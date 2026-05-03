using OctaNotes.Scripts.Play.Model.Interface;
using TMPro;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Result.View
{
    public class SongInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI songName;
        [SerializeField] private TextMeshProUGUI songComposer;
        
        private IGlobalPlayResultContext _globalPlayResultContext;
        
        [Inject]
        public void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext = globalPlayResultContext;
        }

        private void Start()
        {
            SetSongInfo(
                _globalPlayResultContext.SongData.songName, 
                _globalPlayResultContext.SongData.composerName);
        }

        private void SetSongInfo(string title, string composer)
        {
            songName.text = title;
            songComposer.text = composer;
        }
    }
}
