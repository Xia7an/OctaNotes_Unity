using UnityEngine;

namespace OctaNotes.Scripts.Title.Model.Effects
{
    /// <summary>
    /// フェードイン/アウト演出のサンプル実装
    /// CanvasGroupのalphaを操作する
    /// </summary>
    public class FadeEffect : EffectBase
    {
        private readonly CanvasGroup _targetCanvasGroup;
        private readonly EffectParameter<float> _alphaParam;

        /// <summary>
        /// フェード演出を作成
        /// </summary>
        /// <param name="target">対象のCanvasGroup</param>
        /// <param name="startTime">開始時刻</param>
        /// <param name="duration">所要時間</param>
        /// <param name="fromAlpha">開始時のアルファ値</param>
        /// <param name="toAlpha">終了時のアルファ値</param>
        public FadeEffect(
            CanvasGroup target,
            float startTime,
            float duration,
            float fromAlpha,
            float toAlpha)
            : base(startTime, duration)
        {
            _targetCanvasGroup = target;
            _alphaParam = new EffectParameter<float>(fromAlpha, toAlpha, Interpolators.LerpFloat);
            RegisterParameter(_alphaParam);
        }

        protected override void OnStart()
        {
            _targetCanvasGroup.alpha = _alphaParam.InitialValue;
            Debug.Log($"[FadeEffect] Started: alpha {_alphaParam.InitialValue} -> {_alphaParam.FinalValue}");
        }

        protected override void OnUpdate(float progress)
        {
            _targetCanvasGroup.alpha = _alphaParam.CurrentValue;
        }

        protected override void OnComplete()
        {
            _targetCanvasGroup.alpha = _alphaParam.FinalValue;
            Debug.Log($"[FadeEffect] Completed: alpha = {_alphaParam.FinalValue}");
        }
    }
}
