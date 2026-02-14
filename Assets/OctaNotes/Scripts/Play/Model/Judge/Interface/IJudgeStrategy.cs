using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IJudgeStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note">判定対象のノーツ</param>
        /// <param name="buttonStates">各レーンのボタンの状態</param>
        /// <param name="longPushedRate">(ロングノーツ終点のみ)ロングノーツが押された割合</param>
        /// <returns></returns>
        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate);
    }
}
