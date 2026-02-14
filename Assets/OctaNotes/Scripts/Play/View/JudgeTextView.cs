using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class JudgeTextView : MonoBehaviour
    {
        [SerializeField] private Sprite perfectText;
        [SerializeField] private Sprite goodText;
        [SerializeField] private Sprite badText;
        [SerializeField] private Sprite missText;
        [SerializeField] private Image judgeText;
        
        private ILaneViewModel _laneViewModel;

        [Inject]
        public void Construct([InjectOptional] ILaneViewModel laneViewModel = null)
        {
            if (_laneViewModel == null && laneViewModel != null)
            {
                _laneViewModel = laneViewModel;
            }
        }

        private void Start()
        {
            // 透明にする
            judgeText.color = new Color(1, 1, 1, 0);
            _laneViewModel.CurrentJudge.SubscribeAwait(async (v,ct) =>
            {
                await ShowJudgeResult(v,ct);
            }, AwaitOperation.Switch).AddTo(this);
        }

        private async UniTask ShowJudgeResult(Judge judge, CancellationToken token)
        {
            var sprite = judge switch
            {
                Judge.Perfect => perfectText,
                Judge.Good => goodText,
                Judge.Bad => badText,
                Judge.Miss => missText,
                _ => null
            };
            if (sprite == null) return;
            judgeText.color = new Color(1, 1, 1, 1);
            judgeText.sprite = sprite;
            var rect = judgeText.rectTransform;
            rect.sizeDelta = new Vector2(
                sprite.rect.width / 2,
                sprite.rect.height / 2
            );
            await judgeText.DOFade(0, 0.2f).ToUniTask(cancellationToken: token);
        }
    }
}
