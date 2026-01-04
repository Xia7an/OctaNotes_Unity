using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Judgment;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    /// <summary>
    /// 入力コントローラーのインターフェース
    /// </summary>
    public interface IInputController
    {
        /// <summary>
        /// 指定レーンのボタン入力イベントを取得する
        /// </summary>
        /// <param name="laneIndex">レーンインデックス</param>
        /// <param name="currentTime">現在時刻</param>
        /// <returns>ボタン入力イベント</returns>
        ButtonInputEvent GetButtonInput(int laneIndex, float currentTime);

        /// <summary>
        /// ボタン状態のReactivePropertyリスト
        /// </summary>
        List<ReactiveProperty<bool>> IsButtonPressing { get; }
    }
}
