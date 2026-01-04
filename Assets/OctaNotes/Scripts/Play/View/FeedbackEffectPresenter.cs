using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    /// <summary>
    /// フィードバックエフェクトの表示管理
    /// ボタン押下時のレーンハイライトなど
    /// 各レーンのGameObjectにアタッチして使用
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class FeedbackEffectPresenter : MonoBehaviour, IEffectPresenter
    {
        [SerializeField] private int laneIndex = 0; // このPresenterが担当するレーン番号
        [SerializeField] private AudioClip hitSound; // ヒット時のサウンド（オプション）

        private Material material;
        private AudioSource audioSource;
        private CancellationTokenSource cts;

        private void Awake()
        {
            material = GetComponent<MeshRenderer>().material;
            audioSource = GetComponent<AudioSource>();
            cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        public void PresentJudgment(JudgmentEvent judgmentEvent)
        {
            // 自分の担当レーンでない場合は無視
            if (judgmentEvent.NotePosition != laneIndex)
            {
                return;
            }

            // フィードバックエフェクト（レーンハイライトなど）
            // 判定結果がNoneの場合のみフィードバックを表示
            // Perfect/Good/Bad/Missの場合はJudgmentEffectPresenterが処理
            if (judgmentEvent.Result == JudgmentResult.None)
            {
                HighlightLane().Forget();
            }
        }

        private async UniTask HighlightLane()
        {
            Debug.Log($"[FeedbackEffect] Highlight lane {laneIndex}");

            // レーンハイライト処理
            material.SetFloat("_Brighten", 1f);

            // サウンド再生（オプション）
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // フェードアウト
            await material.DOFloat(0f, "_Brighten", 0.2f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(cts.Token);
        }
    }
}
