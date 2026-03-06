using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class Reducer : IReducer
    {
        private const float NOTESPEED_INTERVAL = 0.5f;
        
        private const float JUDGEOFFSET_INTERVAL = 0.1f;
        
        public UIState Reduce(UIState oldState, UIAction action)
        {
            return action switch
            {
                DoNothing => oldState,
                ChangeNoteSpeed(var v)  => oldState with
                {
                    noteSpeed = oldState.noteSpeed + (int)v * NOTESPEED_INTERVAL
                },
                ChangeJudgeOffset(var v) => oldState with
                {
                    judgeOffset = oldState.judgeOffset + (int)v  * JUDGEOFFSET_INTERVAL
                },
                ChangeControlTarget(var v) => oldState with
                {
                    controlTarget = v
                },
            };
        }
    }
}
