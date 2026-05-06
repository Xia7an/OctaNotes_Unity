using System;
using System.IO;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.ViewModel.Interface;
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
        private IMusicViewModel _musicViewModel;
        
        private AudioClip _musicClip;
        private AudioSource _audioSource;

        [Inject]
        private void Construct(IMusicViewModel musicViewModel)
        {
            _musicViewModel = musicViewModel;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            Observable.FromEvent(
                    h => _musicViewModel.OnMusicStart += h,
                    h => _musicViewModel.OnMusicStart -= h)
                .Subscribe(_ =>
                {
                    _musicClip = _musicViewModel.AudioClip.Value;
                    Play();
                })
                .AddTo(this);
        }

        private void Play()
        {
            if (_musicClip == null)
            {
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Music clip is null.", this);
                return;
            }

            _audioSource.clip = _musicClip;
            _audioSource.Play();
        }

        
    }
}
