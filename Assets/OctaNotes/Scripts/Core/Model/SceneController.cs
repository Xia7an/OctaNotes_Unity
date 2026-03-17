using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace OctaNotes.Scripts.Core.Model
{
    public class SceneController: ISceneController, IInitializable
    {
        public event Func<UniTask> OnSceneLoadBegin;
        public event Func<UniTask> OnSceneLoadEnd;
        
        private Scenes _scene;
        
        public void Initialize()
        {
            var activeSceneName = SceneManager.GetActiveScene().name;
            if (System.Enum.TryParse(activeSceneName, true, out Scenes currentScene))
            {
                _scene = currentScene;
                return;
            }

            _scene = Scenes.Title;
            Debug.LogWarning($"SceneController.Initialize: Unknown scene name '{activeSceneName}'. Fallback to {_scene}.");
        }
        public async UniTask ChangeScene(Scenes targetScene)
        {
            if (OnSceneLoadBegin != null)
                await UniTask.WhenAll(Array.ConvertAll(OnSceneLoadBegin.GetInvocationList(), d => ((Func<UniTask>)d)()));
            
            await SceneManager.LoadSceneAsync(targetScene.ToString());
            
            if (OnSceneLoadEnd != null)
                await UniTask.WhenAll(Array.ConvertAll(OnSceneLoadEnd.GetInvocationList(), d => ((Func<UniTask>)d)()));
        }

    }
}
