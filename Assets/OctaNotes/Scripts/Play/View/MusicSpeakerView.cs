using System;
using System.IO;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicSpeakerView : MonoBehaviour
    {
        private IInGameTimer _timer;
        private IGlobalSongDataContext _context;

        private AudioClip _musicClip;
        private AudioSource _audioSource;

        [Inject]
        private void Construct(IInGameTimer timer, IGlobalSongDataContext context)
        {
            _timer = timer;
            _context = context;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _musicClip = CreateAudioClip(_context.MusicPath);
            Observable.FromEvent(
                    h => _timer.OnMusicStart += h,
                    h => _timer.OnMusicStart -= h)
                .Subscribe(_ => Play());
        }

        private void Play()
        {
            if (_musicClip == null)
            {
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Music clip is null. path={_context.MusicPath}", this);
                return;
            }

            _audioSource.clip = _musicClip;
            _audioSource.Play();
        }

        // audioPathで指定された音源ファイルを読み込み、AudioClipにして返す関数
        private AudioClip CreateAudioClip(string audioPath)
        {
            if (string.IsNullOrWhiteSpace(audioPath) || !File.Exists(audioPath))
            {
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Audio file not found: {audioPath}", this);
                return null;
            }

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
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Failed to load audio clip: {audioPath}, error={request.error}", this);
                return null;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Loaded clip is null: {audioPath}", this);
                return null;
            }

            clip.name = Path.GetFileNameWithoutExtension(audioPath);
            return clip;
        }
    }
}
