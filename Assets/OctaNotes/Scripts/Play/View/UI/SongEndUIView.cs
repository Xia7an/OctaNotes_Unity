using System;
using System.Runtime.CompilerServices.Core.View.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using OctaNotes.Scripts.Utils;
using R3;
using TMPro;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View.UI
{
    public class SongEndUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI APText;
        
        private ISongEndUIViewModel _songEndUIViewModel;

        [Inject]
        private void Construct(ISongEndUIViewModel songEndUIViewModel)
        {
            _songEndUIViewModel = songEndUIViewModel;
        }

        private void Start()
        {
            _songEndUIViewModel.ShowClearMark.Skip(1)
                // .Where(v => v is ClearMark.Ap)
                .SubscribeAwait((v, ct) => ShowApfc(v, ct)).AddTo(this);
        }
        
        private async UniTask ShowApfc(ClearMark mark, CancellationToken ct)
        {
            var text = mark switch
            {
                ClearMark.Ap => "ALL PERFECT !",
                ClearMark.Fc => "FULL COMBO !",
                ClearMark.Clear => "CLEAR",
                ClearMark.Failed => "FAILED...",
                _ => "F*ck you"
            };

            APText.text = text;                         // テキストの反映
        
            var color = mark switch
            {
                ClearMark.Ap     => (ColorUtils.GetColor("#FFED00"), ColorUtils.GetColor("#003BFF")),
                ClearMark.Fc     => (ColorUtils.GetColor("#B4BAF1"), ColorUtils.GetColor("#11297B")),
                ClearMark.Clear  => (ColorUtils.GetColor("#FFFFFF"), ColorUtils.GetColor("#3A3A3A")),
                ClearMark.Failed => (ColorUtils.GetColor("#3F3030"), ColorUtils.GetColor("#000000")),
                _ => throw new ArgumentOutOfRangeException(nameof(mark), mark, null)
            };
            
            APText.SetGradientColor(color.Item1, color.Item2, false);
            
            APText.transform.localScale = Vector3.zero; // スケール 0
            APText.characterSpacing = -70f;             // characterSpacing -70
            
            // 拡張メソッドを使って初期不透明度を0に設定
            APText.SetGradientAlpha(0f);

            // 2. シーケンスの作成
            Sequence seq = DOTween.Sequence();

            // --- フェーズ1 (0.2秒) ---
            // スケールを0から1、不透明度を0から1、characterSpacingを-70から10にする
            seq.Append(APText.transform.DOScale(Vector3.one, 0.2f));
            seq.Join(APText.DOFadeGradient(1f, 0.2f)); // 拡張メソッドを利用
            seq.Join(DOTween.To(
                getter: () => APText.characterSpacing,
                setter: x => APText.characterSpacing = x,
                endValue: 10f,
                duration: 0.2f
            ));

            // --- フェーズ2 (1.5秒) ---
            // characterSpacingを10から20にする
            seq.Append(DOTween.To(
                getter: () => APText.characterSpacing,
                setter: x => APText.characterSpacing = x,
                endValue: 20f,
                duration: 1.5f
            ).SetEase(Ease.Linear));

            // --- フェーズ3 (0.2秒) ---
            // characterSpacingを20から100、スケールを1から4、不透明度を1から0にする
            seq.Append(APText.transform.DOScale(Vector3.one * 4f, 0.2f));
            seq.Join(APText.DOFadeGradient(0f, 0.2f)); // 拡張メソッドを利用
            seq.Join(DOTween.To(
                getter: () => APText.characterSpacing,
                setter: x => APText.characterSpacing = x,
                endValue: 100f,
                duration: 0.2f
            ));

            // 3. アニメーションの完了を待機
            await seq.WithCancellation(ct);
        }
        
       
    }
}
