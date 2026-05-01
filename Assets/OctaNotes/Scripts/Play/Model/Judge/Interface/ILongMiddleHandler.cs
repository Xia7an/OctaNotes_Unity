using R3;
using System;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface ILongMiddleHandler
    {
        /// <summary>
        /// ロング視点から終点までの何割ボタンが押されていたかを返す
        /// </summary>
        public ReactiveProperty<float> LongPushedRate { get; }
        
        /// <summary>
        /// 現在判定中のロングノーツがあり、そのロングノーツが押されているかどうかを保持する
        /// </summary>
        ReactiveProperty<bool> IsPushedLongNote { get; }

        /// <summary>
        /// 指定したロング終点Guidに対応する押下率が確定済みなら取得する
        /// </summary>
        bool TryGetLongEndPushedRate(Guid longEndGuid, out float pushedRate);

        /// <summary>
        /// 判定直前に最新ノーツ状態を同期する
        /// </summary>
        void SyncWithCurrentNote(Note note);

        /// <summary>
        /// ロング始点の判定確定時にロング計測を開始する
        /// </summary>
        void NotifyLongStartJudged(Note note);
        
        
    }
}
