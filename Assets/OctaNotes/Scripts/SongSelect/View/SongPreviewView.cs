using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.View;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class SongPreviewView : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        
        private IUIState _uiState;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Select(v => v.cursor.songIndex).DistinctUntilChanged().SubscribeAwait(async (v, ct) =>
            {
                await UniTask.WaitForSeconds(1, cancellationToken: ct);
                var path = _uiState.State.Value.songDataList[v].musicPath;
                await PlaySong(path, ct);
            }, AwaitOperation.Switch).AddTo(this);
        }

        private async UniTask PlaySong(string songPath, CancellationToken ct)
        {
            await _audioSource.DOFade(0, 0.2f).ToUniTask(cancellationToken: ct);
            _audioSource.Stop();
            var audioClip = CreateAudioClip(songPath);
            _audioSource.clip = audioClip;
            _audioSource.Play();
            await _audioSource.DOFade(1, 0.2f).ToUniTask(cancellationToken: ct);
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
