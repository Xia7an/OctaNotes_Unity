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
                    cursor = oldState.cursor with
                    {
                        songIndex = MoveClamped(
                            oldState.selectedSongIndex,
                            v,
                            oldState.songDataList?.Count ?? 0)
                    }
                },
                SelectDifficulty(var v) => oldState with
                {
                    selectedDifficulty = CalcDifficulty(oldState.selectedDifficulty, v)
                },
                SelectOption(var v) => oldState with
                {
                    cursor = oldState.cursor with
                    {
                        optionIndex = MoveCyclic(
                            (int)oldState.selectedOptions,
                            v,
                            Enum.GetValues(typeof(Options)).Length)
                    }
                },
                ChangeNoteSpeed(var v) => oldState with
                {
                    noteSpeed = Math.Clamp(oldState.noteSpeed + (int)v * 0.5, 1.0, 20.0)
                },
                ChangeJudgeOffset(var v) => oldState with
                {
                    judgeOffsetMs = Math.Clamp(oldState.judgeOffsetMs + (int)v*10, -500, 500)
                },
                ReloadSongList(var v) => oldState with
                {
                    songDataList = new List<SongData>(v),
                    cursor = oldState.cursor with
                    {
                        songIndex = MoveClamped(oldState.selectedSongIndex, Direction.Down, v.Length)
                    }
                },
                ConfirmSong => oldState with
                {
                    songConfirmed = true
                }
            };
        }

        private static int MoveClamped(int current, Direction direction, int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            var next = current - (int)direction;
            return Math.Clamp(next, 0, count - 1);
        }

        private static int MoveCyclic(int current, Direction direction, int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            var next = current - (int)direction;
            return (next % count + count) % count;
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
