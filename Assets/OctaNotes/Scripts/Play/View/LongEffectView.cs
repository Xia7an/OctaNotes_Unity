using OctaNotes.Scripts.Play.Model.Interface;
using R3;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class LongEffectView : MonoBehaviour
    {
        private ILongMiddleHandler _longMiddleHandler;
        
        [SerializeField] private VisualEffect longEffect;

        [Inject]
        public void Construct([InjectOptional] ILongMiddleHandler longMiddleHandler = null)
        {
            if (longMiddleHandler != null)
            {
                _longMiddleHandler = longMiddleHandler;
            }
        }

        private void Start()
        {
            if (_longMiddleHandler == null)
            {
                Debug.LogWarning($"[{nameof(LongEffectView)}] ILongMiddleHandler is not injected on {gameObject.name}. Skipping Long VFX subscription.", this);
                return;
            }

            _longMiddleHandler.IsPushedLongNote.Subscribe(HandleVFX).AddTo(this);
        }

        private void HandleVFX(bool isPushed)
        {
            longEffect.SendEvent(isPushed ? "OnStart" : "OnStop");
        }
    }
}
