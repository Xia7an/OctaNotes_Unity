using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Title.View
{
    [RequireComponent(typeof(AudioSource))]
    public class GameStartSEView : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        
        private ISceneController _sceneController;

        [Inject]
        private void Construct(ISceneController sceneController)
        {
            _sceneController = sceneController;
        }

        private void Start()
        {
            Observable.FromEvent(
                    h => new Func<UniTask>(() =>
                    {
                        h();
                        return UniTask.CompletedTask;
                    }),
                    h => _sceneController.OnSceneLoadBegin += h,
                    h => _sceneController.OnSceneLoadBegin -= h)
                .SubscribeAwait((_, ct) => OnSceneLoadBegin())
                .AddTo(this);
        }

        private UniTask OnSceneLoadBegin()
        {
            GetComponent<AudioSource>().PlayOneShot(clip);
            return UniTask.CompletedTask;
        }
    }
}
