using System;
using DefaultNamespace;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // ボタン入力から操作の意味に変換するクラス
    public class InputContextResolver : IInitializable, IDisposable
    {
        private readonly IPlayInputLayer _playInputLayer;
        
        private CompositeDisposable _disposables = new();
        private IDispachable _dispachable;
        private IUIState _state;

        public void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                var i1 = i; // Subscribeの中身がラムダ式であり、変数がそのままキャプチャされるため、各講読で独立した変数が割り当てられるように別の変数を定義している。
                _playInputLayer.IsButtonPressing[i]
                    .Where(v => v is ButtonState.BeginPush)
                    .Subscribe(_ => OnPhysicalButtonPressed(i1)).AddTo(_disposables);
            }
        }
        public void Dispose()
        {
            _disposables?.Dispose();
        }

        public InputContextResolver(IDispachable dispachable)
        {
            _dispachable = dispachable;
        }
        
        
        private void OnPhysicalButtonPressed(int buttonIdx)
        {
            UIAction action = new DoNothing();
            var state = _state.State.Value;
            switch (buttonIdx)
            {
                case 0:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Down),
                        // Target.CategoryList => new SelectCategory(Direction.Down),
                        Target.GameOptions => new SelectOption(Direction.Down)
                    };
                    break;
                
                case 1:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Up),
                        // Target.CategoryList => new SelectCategory(Direction.Up),
                        Target.GameOptions => new SelectOption(Direction.Up)
                    };
                    break;
                
                case 2:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new ConfirmSong(),
                        _ => new ChangeControlTarget(Target.SongList)
                    };
                    break;
                
                case 3:
                    if (state.controlTarget == Target.SongList) action = new ChangeControlTarget(Target.GameOptions);
                    break;
                
                case 4:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectDifficulty(Direction.Down),
                        Target.GameOptions when state.selectedOption is Options.NoteSpeed => new ChangeNoteSpeed(
                            Direction.Down),
                        Target.GameOptions when state.selectedOption is Options.JudgeOffset => new ChangeJudgeOffset(
                            Direction.Down),
                    };
                    break;
                
                case 5:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Up),
                        Target.GameOptions when state.selectedOption is Options.NoteSpeed => new ChangeNoteSpeed(
                            Direction.Up),
                        Target.GameOptions when state.selectedOption is Options.JudgeOffset => new ChangeJudgeOffset(
                            Direction.Up),
                    };
                    break;
                
                case 6:
                    if (state.controlTarget == Target.SongList) action = new ChangeControlTarget(Target.CategoryList);
                    break;
                
                case 7:
                    if (state.controlTarget == Target.SongList) action = new ChangeControlTarget(Target.SongSort);
                    break;
            }
            _dispachable.Dispatch(action);
        }

    }
}
