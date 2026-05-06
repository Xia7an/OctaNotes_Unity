using System;
using System.IO;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class MusicViewModel : IMusicViewModel, IInitializable, IDisposable
    {
        private IGlobalSongDataContext _context;
        private IInGameTimer _timer;

        public ReactiveProperty<AudioClip> AudioClip { get; } =  new ReactiveProperty<AudioClip>();

        public event Action OnMusicStart;
        public event Action OnMusicEnd;

        private CompositeDisposable _disposables = new CompositeDisposable();
        
        private float songLength;
        
        public MusicViewModel(IGlobalSongDataContext context,  IInGameTimer timer)
        {
            _context = context;
            _timer = timer;
        }
        

        public void Initialize()
        {
            AudioClip.Value = CreateAudioClip(_context.MusicPath);
            songLength = AudioClip.Value.length;
            Observable.EveryUpdate()
                .Where(_ => _timer.Time.Value >= songLength)
                .Take(1)
                .Subscribe(_ => OnMusicEnd?.Invoke())
                .AddTo(_disposables);
            
            Observable.FromEvent(
                h => _timer.OnMusicStart += h,
                h => _timer.OnMusicStart -= h)
                .Subscribe(_ => OnMusicStart?.Invoke())
                .AddTo(_disposables);
        }
        
        public void Dispose()
        {
            AudioClip?.Dispose();
            _disposables.Dispose();
        }
        
        // audioPathで指定された音源ファイルを読み込み、AudioClipにして返す関数
        private AudioClip CreateAudioClip(string audioPath)
        {
            var extension = Path.GetExtension(audioPath).ToLowerInvariant();
            var audioType = extension switch
            {
                ".wav" => AudioType.WAV,
                ".ogg" => AudioType.OGGVORBIS,
                ".mp3" => AudioType.MPEG,
                ".aif" or ".aiff" => AudioType.AIFF,
                _ => AudioType.UNKNOWN,
            };

            using var request = UnityWebRequestMultimedia.GetAudioClip(new Uri(audioPath).AbsoluteUri, audioType);
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Failed to load audio clip: {audioPath}, error={request.error}");
                return null;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogWarning($"Loaded clip is null: {audioPath}");
                return null;
            }

            clip.name = Path.GetFileNameWithoutExtension(audioPath);
            return clip;
        }

    }
}
