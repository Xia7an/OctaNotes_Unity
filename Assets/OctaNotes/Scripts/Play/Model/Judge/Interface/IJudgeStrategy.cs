using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IJudgeStrategy
    {
        public JudgeResult JudgeNote(Note note, List<ButtonState> buttonStates, float longPushedRate);
    }
}
