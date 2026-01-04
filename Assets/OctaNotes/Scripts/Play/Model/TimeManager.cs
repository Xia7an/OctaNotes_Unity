using System;
using OctaNotes.Scripts.Play.Interface;
using R3;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// 時間管理システム
    /// </summary>
    public class TimeManager : IGameTime
    {
        private readonly Subject<float> _updateSubject = new Subject<float>();

        public double CurrentTime => Time.time;

        /// <summary>
        /// 現在時刻を取得する
        /// </summary>
        public float GetCurrentTime()
        {
            return Time.time;
        }

        /// <summary>
        /// 更新通知を購読する
        /// </summary>
        public IDisposable SubscribeToUpdate(Action<float> callback)
        {
            return _updateSubject.Subscribe(callback);
        }

        /// <summary>
        /// 更新通知を発行する（毎フレーム呼び出し）
        /// </summary>
        public void PublishUpdate()
        {
            _updateSubject.OnNext(GetCurrentTime());
        }
    }
}
