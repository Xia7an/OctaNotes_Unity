using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Title.Interface;
using R3;
using UnityEngine;

namespace OctaNotes.Scripts.Title.Model
{
    /// <summary>
    /// 演出のスケジューリングと実行を管理する
    /// </summary>
    public class EffectScheduler : IDisposable
    {
        private readonly ISeekableGameTime _gameTime;
        private readonly List<EffectBase> _effects = new List<EffectBase>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public EffectScheduler(ISeekableGameTime gameTime)
        {
            _gameTime = gameTime;

            // 時刻変更イベントを購読
            _gameTime.OnTimeChanged
                .Subscribe(HandleTimeChanged)
                .AddTo(_disposables);
        }

        /// <summary>
        /// 演出を登録する
        /// </summary>
        public void Register(EffectBase effect)
        {
            _effects.Add(effect);
            // 開始時刻順にソート
            _effects.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        }

        /// <summary>
        /// 複数の演出を一括登録
        /// </summary>
        public void RegisterRange(IEnumerable<EffectBase> effects)
        {
            foreach (var effect in effects)
            {
                Register(effect);
            }
        }

        /// <summary>
        /// 毎フレーム呼び出して演出を更新する
        /// </summary>
        public void Tick()
        {
            float currentTime = (float)_gameTime.CurrentTime;

            foreach (var effect in _effects)
            {
                if (effect.IsCompleted) continue;

                effect.Update(currentTime);
            }
        }

        /// <summary>
        /// 時刻が強制変更された時の処理
        /// </summary>
        private void HandleTimeChanged(TimeChangedEvent e)
        {
            float newTime = (float)e.NewTime;

            Debug.Log($"[EffectScheduler] Time changed: {e.PreviousTime:F3} -> {e.NewTime:F3}");

            // 新しい時刻より前に終了するはずの演出を強制完了
            foreach (var effect in _effects)
            {
                if (effect.IsCompleted) continue;

                // 新しい時刻が演出の終了時刻を超えている場合
                if (newTime >= effect.EndTime)
                {
                    effect.ForceComplete();
                }
            }
        }

        /// <summary>
        /// 全演出をクリア
        /// </summary>
        public void Clear()
        {
            _effects.Clear();
        }

        /// <summary>
        /// 未完了の演出数を取得
        /// </summary>
        public int PendingEffectCount => _effects.FindAll(e => !e.IsCompleted).Count;

        public void Dispose()
        {
            _disposables.Dispose();
            _effects.Clear();
        }
    }
}
