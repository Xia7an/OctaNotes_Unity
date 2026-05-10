using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using UnityEngine;
using Zenject;
using R3;

namespace OctaNotes.Scripts.Play.View
{
    [RequireComponent(typeof(AudioSource))]
    public class JudgeSoundView : MonoBehaviour, ILaneView
    {
        
        [SerializeField] private AudioClip perfectClip;
        [SerializeField] private AudioClip goodClip;
        [SerializeField] private AudioClip badClip;
        [SerializeField] private AudioClip noneClip;
        [SerializeField] private AudioClip exPerfectClip;

        private IJudgeSoundViewModel _judgeSoundViewModel;
        
        private AudioSource _audioSource;

        [Inject]
        private void Construct([InjectOptional] IJudgeSoundViewModel judgeSoundViewModel)
        {
            if (_judgeSoundViewModel == null && judgeSoundViewModel != null)
            {
                _judgeSoundViewModel = judgeSoundViewModel;
            }
        }
        

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (_judgeSoundViewModel == null)
            {
                Debug.LogWarning($"[{nameof(JudgeSoundView)}] IJudgeSoundViewModel is not injected on {gameObject.name}.", this);
                return;
            }

            _judgeSoundViewModel.JudgeForSound.Subscribe(PlayJudgeSound).AddTo(this);
        }

        private void PlayJudgeSound(JudgeSound judgesound)
        {
            if(judgesound.judge == Judge.NotJudged) return;
            var clip = judgesound.judge switch
            {
                Judge.Perfect when !judgesound.isEx => perfectClip,
                Judge.Perfect when judgesound.isEx => exPerfectClip,
                Judge.Good => goodClip,
                Judge.Bad => badClip,
                Judge.None => noneClip,
                _  => null
            };
            _audioSource.PlayOneShot(clip);
            // Debug.Log("Played Judge sound.");
        }
    }
}
