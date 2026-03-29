using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    [RequireComponent(typeof(AudioSource))]
    public class SongSelectSoundView : MonoBehaviour
    {
        [SerializeField] private AudioClip songCursorSound;
        [SerializeField] private AudioClip confirmSongSound;
        
        private IUIState _uiState;
        private ISongSelectActionEventSource _actionSource;
        
        private AudioSource _audioSource;
        
        [Inject]
        private void Construct(IUIState uiState,  ISongSelectActionEventSource actionSource)
        {
            _uiState = uiState;
            _actionSource = actionSource;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _uiState.State.Select(v => v.selectedSongIndex)
                .Subscribe(_ => _audioSource.PlayOneShot(songCursorSound))
                .AddTo(this);

            Observable.FromEvent<UIAction>(
                    h => _actionSource.OnActionDispatched += h,
                    h => _actionSource.OnActionDispatched -= h
                )
                .Where(v => v is ConfirmSong)
                .Subscribe(_ => _audioSource.PlayOneShot(confirmSongSound))
                .AddTo(this);
        }
    }
}
