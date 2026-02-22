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

        private const float YDifference = 10;
        
        private ILaneViewModel _laneViewModel;
        
        private Vector2 _initialPosition;
        
        
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
            
            _initialPosition = judgeText.rectTransform.anchoredPosition;
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
            
            // 色と座標を初期化
            judgeText.color = new Color(1, 1, 1, 1);
            judgeText.rectTransform.anchoredPosition = _initialPosition;
            
            judgeText.sprite = sprite;
            var rect = judgeText.rectTransform;
            rect.sizeDelta = new Vector2(
                sprite.rect.width / 2,
                sprite.rect.height / 2
            );
            await UniTask.WhenAll(
                judgeText.DOFade(0, 0.2f).ToUniTask(cancellationToken: token),
                judgeText.rectTransform.DOAnchorPosY(_initialPosition.y + YDifference, 0.2f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token)
            );
        }
    }
}
