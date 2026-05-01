using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
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
            _songEndUIViewModel.ShowClearMark
                // .Where(v => v is ClearMark.Ap)
                .SubscribeAwait((v, ct) => ShowApfc(v, ct)).AddTo(this);
        }
        
        private async UniTask ShowApfc(ClearMark mark, CancellationToken ct)
        {
            var text = mark switch
            {
                ClearMark.Ap => "ALL PERFECT",
                ClearMark.Fc => "FULL COMBO",
                ClearMark.Clear => "Clear",
                ClearMark.Failed => "落単...",
                _ => "F*ck you"
            };

            APText.text = text;                         // テキストの反映
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

    // =========================================================
    // 純粋な拡張メソッド群 (別のスクリプトファイルに分けてもOKです)
    // =========================================================
    public static class TextMeshProUGUIExtensions
    {
        /// <summary>
        /// TextMeshProのGradientのアルファ値を即座に設定する
        /// </summary>
        public static void SetGradientAlpha(this TextMeshProUGUI target, float alpha)
        {
            VertexGradient vg = target.colorGradient;
            vg.topLeft.a = alpha;
            vg.topRight.a = alpha;
            vg.bottomLeft.a = alpha;
            vg.bottomRight.a = alpha;
            target.colorGradient = vg;
        }

        /// <summary>
        /// TextMeshProのGradientのアルファ値をTweenさせる (DOTween標準のDOFadeと同等の使用感)
        /// </summary>
        public static Tween DOFadeGradient(this TextMeshProUGUI target, float endValue, float duration)
        {
            return DOTween.To(
                getter: () => target.colorGradient.topLeft.a,
                setter: alpha =>
                {
                    VertexGradient vg = target.colorGradient;
                    vg.topLeft.a = alpha;
                    vg.topRight.a = alpha;
                    vg.bottomLeft.a = alpha;
                    vg.bottomRight.a = alpha;
                    target.colorGradient = vg;
                },
                endValue: endValue,
                duration: duration
            ).SetTarget(target); // Tweenのターゲットを明示してKillしやすくしておく
        }
    }
}
