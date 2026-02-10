using System;
using UnityEngine;

namespace OctaNotes.Scripts.Title.Model
{
    /// <summary>
    /// 演出パラメータの補間インターフェース
    /// </summary>
    public interface IEffectParameter
    {
        /// <summary>
        /// 進捗率（0-1）に基づいて現在値を更新
        /// </summary>
        void SetProgress(float progress);

        /// <summary>
        /// 強制的に終了値に設定
        /// </summary>
        void ForceComplete();

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// 初期値と終了値を持つ演出パラメータ
    /// </summary>
    public class EffectParameter<T> : IEffectParameter
    {
        public T InitialValue { get; }
        public T FinalValue { get; }
        public T CurrentValue { get; private set; }

        private readonly Func<T, T, float, T> _interpolator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="initialValue">初期値</param>
        /// <param name="finalValue">終了値</param>
        /// <param name="interpolator">補間関数（初期値, 終了値, 進捗率 => 現在値）</param>
        public EffectParameter(T initialValue, T finalValue, Func<T, T, float, T> interpolator)
        {
            InitialValue = initialValue;
            FinalValue = finalValue;
            CurrentValue = initialValue;
            _interpolator = interpolator;
        }

        public void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            CurrentValue = _interpolator(InitialValue, FinalValue, progress);
        }

        public void ForceComplete()
        {
            CurrentValue = FinalValue;
        }

        public void Reset()
        {
            CurrentValue = InitialValue;
        }
    }

    /// <summary>
    /// よく使う補間関数を提供するヘルパー
    /// </summary>
    public static class Interpolators
    {
        public static float LerpFloat(float a, float b, float t) => Mathf.Lerp(a, b, t);
        public static Vector3 LerpVector3(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
        public static Color LerpColor(Color a, Color b, float t) => Color.Lerp(a, b, t);
        public static Quaternion LerpQuaternion(Quaternion a, Quaternion b, float t) => Quaternion.Lerp(a, b, t);
    }
}
