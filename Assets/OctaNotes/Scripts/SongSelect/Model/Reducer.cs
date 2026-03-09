using System;
using OctaNotes.Scripts.Core.Model;
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

        private readonly Category[] _categories = new Category[] { Category.Pops , Category.Vocaloid, Category.Game};
        
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
                SelectSong(var v) => oldState with
                {
                    selectedSongId = v is Direction.Up ?
                        oldState.guidList[oldState.selectedSongId].next : 
                        oldState.guidList[oldState.selectedSongId].prev
                },
                ConfirmSong => oldState with
                {
                    songConfirmed = true
                },
                SelectDifficulty(var v) => oldState with
                {
                    selectedDifficulty = CalcDifficulty(oldState.selectedDifficulty, v)
                },
                SelectOption(var v) => oldState with
                {
                    selectedOption = CalcOptions(oldState.selectedOption, v)
                },
            };
        }

        private Difficulty CalcDifficulty(Difficulty difficulty, Direction direction)
        {
            if (direction is Direction.Up)
                return difficulty < Difficulty.Octa ? difficulty + 1 : Difficulty.Octa;
            else
                return difficulty > Difficulty.Dual ? difficulty - 1 : Difficulty.Dual;
        }

        private Options CalcOptions(Options option, Direction direction)
        {
            if(direction is Direction.Up)
                return option < Options.JudgeOffset ? option + 1 : option;
            else
                return option > Options.JudgeOffset ? option - 1 : option;
        }
    }
}
