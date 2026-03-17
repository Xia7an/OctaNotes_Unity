using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class Reducer : IReducer
    {
        public UIState Reduce(UIState oldState, UIAction action)
        {
            return action switch
            {
                NotAssigned => oldState,
                ChangeControlTarget(var v) => oldState with
                {
                    controlTarget = v
                },
                SelectSong(var v) => oldState with
                {
                    selectedSongIndex = Math.Clamp(
                        oldState.selectedSongIndex + (int)v,
                        0,
                        oldState.songDataList != null && oldState.songDataList.Count > 0
                            ? oldState.songDataList.Count - 1
                            : 0)
                },
                SelectDifficulty(var v) => oldState with
                {
                    selectedDifficulty = CalcDifficulty(oldState.selectedDifficulty, v)
                },
                SelectOption(var v) => oldState with
                {
                    selectedOptions =(Options)(((int)oldState.selectedOptions +  (int)v) % 2)
                },
                ReloadSongList(var v) => oldState with
                {
                    songDataList = new List<SongData>(v)
                }
            };
        }

        private Difficulty CalcDifficulty(Difficulty difficulty, Direction direction)
        {
            if (direction is Direction.Up)
                return difficulty < Difficulty.Octa ? difficulty + 1 : Difficulty.Octa;
            else
                return difficulty > Difficulty.Dual ? difficulty - 1 : Difficulty.Dual;
        }
    }
}
