using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
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

        private IJudgeSoundViewModel _judgeSoundViewModel;
        private AudioSource _audioSource;

        [Inject]
        public void Construct(IJudgeSoundViewModel judgeSoundViewModel)
        {
            _judgeSoundViewModel = judgeSoundViewModel;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _judgeSoundViewModel.JudgeForSound.Subscribe(PlayJudgeSound).AddTo(this);
        }

        private void PlayJudgeSound(Judge judge)
        {
            if(judge == Judge.NotJudged) return;
            var clip = judge switch
            {
                Judge.Perfect => perfectClip,
                Judge.Good => goodClip,
                Judge.Bad => badClip,
                Judge.None => noneClip,
                _  => null
            };
            _audioSource.PlayOneShot(clip);
        }
    }
}
