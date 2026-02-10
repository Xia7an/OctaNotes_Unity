using System.Collections.Generic;
using UnityEngine;

namespace OctaNotes.Scripts.Title.Model
{
    /// <summary>
    /// 演出の状態
    /// </summary>
    public enum EffectState
    {
        Pending,    // 開始前
        Running,    // 実行中
        Completed   // 完了
    }

    /// <summary>
    /// 演出の抽象基底クラス
    /// </summary>
    public abstract class EffectBase
    {
        /// <summary>
        /// 演出開始時刻（秒）
        /// </summary>
        public float StartTime { get; }

        /// <summary>
        /// 演出の所要時間（秒）
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// 演出終了時刻
        /// </summary>
        public float EndTime => StartTime + Duration;

        /// <summary>
        /// 現在の状態
        /// </summary>
        public EffectState State { get; private set; } = EffectState.Pending;

        /// <summary>
        /// 完了済みかどうか
        /// </summary>
        public bool IsCompleted => State == EffectState.Completed;

        /// <summary>
        /// この演出が管理するパラメータ一覧
        /// </summary>
        protected readonly List<IEffectParameter> Parameters = new List<IEffectParameter>();

        protected EffectBase(float startTime, float duration)
        {
            StartTime = startTime;
            Duration = duration;
        }

        /// <summary>
        /// 時刻に基づいて演出を更新する（EffectSchedulerから呼び出される）
        /// </summary>
        public void Update(float currentTime)
        {
            if (State == EffectState.Completed) return;

            // まだ開始時刻に達していない
            if (currentTime < StartTime)
            {
                return;
            }

            // 開始処理
            if (State == EffectState.Pending)
            {
                State = EffectState.Running;
                OnStart();
            }

            // 終了時刻を超えた場合は強制完了
            if (currentTime >= EndTime)
            {
                ForceComplete();
                return;
            }

            // 進捗率を計算して更新
            float elapsed = currentTime - StartTime;
            float progress = Mathf.Clamp01(elapsed / Duration);

            foreach (var param in Parameters)
            {
                param.SetProgress(progress);
            }

            OnUpdate(progress);
        }

        /// <summary>
        /// 演出を強制的に完了状態にする
        /// </summary>
        public void ForceComplete()
        {
            if (State == EffectState.Completed) return;

            // 開始していなかった場合は開始処理を呼ぶ
            if (State == EffectState.Pending)
            {
                OnStart();
            }

            // 全パラメータを終了値に設定
            foreach (var param in Parameters)
            {
                param.ForceComplete();
            }

            State = EffectState.Completed;
            OnComplete();

            Debug.Log($"[EffectBase] Effect completed: StartTime={StartTime}, Duration={Duration}");
        }

        /// <summary>
        /// パラメータを登録する
        /// </summary>
        protected void RegisterParameter(IEffectParameter parameter)
        {
            Parameters.Add(parameter);
        }

        /// <summary>
        /// 演出開始時に呼ばれる
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// 演出更新時に呼ばれる
        /// </summary>
        /// <param name="progress">進捗率（0-1）</param>
        protected abstract void OnUpdate(float progress);

        /// <summary>
        /// 演出完了時に呼ばれる
        /// </summary>
        protected abstract void OnComplete();
    }
}
