using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicSpeakerView : MonoBehaviour
    {
        private IMusicViewModel _musicViewModel;
        private IInGameTimer _timer;

        private AudioClip _musicClip;
        private AudioSource _audioSource;

        [Inject]
        private void Construct(IMusicViewModel musicViewModel, IInGameTimer timer)
        {
            _musicViewModel = musicViewModel;
            _timer = timer;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            // タイマー初期化時にDSPクロック基準の再生開始時刻が確定するので、
            // PlayScheduled で正確なタイミングに再生を予約する
            Observable.FromEvent<double>(
                    h => _timer.OnTimerInitialized += h,
                    h => _timer.OnTimerInitialized -= h)
                .Subscribe(dspTime =>
                {
                    _musicClip = _musicViewModel.AudioClip.Value;
                    PlayScheduled(dspTime);
                })
                .AddTo(this);
        }

        private void PlayScheduled(double dspTime)
        {
            if (_musicClip == null)
            {
                Debug.LogWarning($"[{nameof(MusicSpeakerView)}] Music clip is null.", this);
                return;
            }

            _audioSource.clip = _musicClip;
            _audioSource.PlayScheduled(dspTime);
        }
    }
}
