using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
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
    // 物理的なボタン入力からユーザーの行いたい操作の意味に変換するクラス
    public class InputContextResolver : IInitializable, IDisposable
    {
        private readonly IInputLayer inputLayer;
        private readonly IUIState _state;
        private readonly IDispachable _dispachable;
        private readonly ISongSelectActionDispatchable actionDispatchable;

        private CompositeDisposable _disposables = new();

        public InputContextResolver(
            IInputLayer inputLayer,
            IDispachable dispachable,
            IUIState state,
            ISongSelectActionDispatchable actionDispatchable)
        {
            this.inputLayer = inputLayer;
            _dispachable = dispachable;
            this.actionDispatchable = actionDispatchable;
            _state = state;
        }

        public void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                var i1 = i; // Subscribeの中身がラムダ式であり、変数がそのままキャプチャされるため、各購読で独立した変数が割り当てられるように別の変数を定義している。
                inputLayer.IsButtonPressing[i]
                    .Where(v => v is ButtonState.BeginPush)
                    .Subscribe(_ => OnPhysicalButtonPressed(i1)).AddTo(_disposables);
                
                // 長押しでイベントが連続発火するための実装
                inputLayer.IsButtonPressing[i]
                    .Where(v => v is ButtonState.Pushed)
                    .Select(_ => 
                        Observable.Timer(
                                TimeSpan.FromSeconds(0.25), // 初回は0.25秒後に発火
                                TimeSpan.FromSeconds(0.1) // 0.1秒間隔で実行 
                            )
                            // 2. ボタンの状態が Pushed 以外になったらタイマーをキャンセルして終了する
                            .TakeUntil(inputLayer.IsButtonPressing[i1].Where(state => state is not ButtonState.Pushed))
                    )
                    .Switch()
                    .Subscribe(_ =>
                    {
                        OnPhysicalButtonPressed(i1);
                    })
                    // 5. 破棄の紐づけ（自身のコンポーネントやCancellationTokenを指定してください）
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void OnPhysicalButtonPressed(int buttonIdx)
        {
            var state = _state.State.Value;
            switch (buttonIdx)
            {
                case 0:
                {
                    UIAction action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Down),
                        // Target.CategoryList => new SelectCategory(Direction.Down),
                        Target.GameOptions => new SelectOption(Direction.Down),
                        _ => new NotAssigned()
                    };
                    DispatchAction(action, state);
                    break;
                }

                case 1:
                {
                    UIAction action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Up),
                        // Target.CategoryList => new SelectCategory(Direction.Up),
                        Target.GameOptions => new SelectOption(Direction.Up),
                        _ => new NotAssigned()
                    };
                    DispatchAction(action, state);
                    break;
                }

                case 2:
                {
                    UIAction action = state.controlTarget switch
                    {
                        Target.SongList => new ConfirmSong(),
                        _ => new ChangeControlTarget(Target.SongList)
                    };
                    DispatchAction(action, state);
                    break;
                }

                case 3:
                {
                    if (state.controlTarget == Target.SongList)
                        DispatchAction(new ChangeControlTarget(Target.GameOptions), state);
                    break;
                }

                case 4:
                {
                    UIAction action = state.controlTarget switch
                    {
                        Target.SongList => new SelectDifficulty(Direction.Down),
                        Target.GameOptions => state.selectedOptions switch
                        {
                            Options.NoteSpeed => new ChangeNoteSpeed(Direction.Down),
                            Options.JudgeOffset => new ChangeJudgeOffset(Direction.Down),
                            _ => new NotAssigned()
                        },
                        _ => new NotAssigned()
                    };
                    DispatchAction(action, state);
                    break;
                }

                case 5:
                {
                    UIAction action = state.controlTarget switch
                    {
                        Target.SongList => new SelectDifficulty(Direction.Up),
                        Target.GameOptions => state.selectedOptions switch
                        {
                            Options.NoteSpeed => new ChangeNoteSpeed(Direction.Up),
                            Options.JudgeOffset => new ChangeJudgeOffset(Direction.Up),
                            _ => new NotAssigned()
                        },
                        _ => new NotAssigned()
                    };
                    DispatchAction(action, state);
                    break;
                }

                case 6:
                {
                    if (state.controlTarget == Target.SongList)
                        DispatchAction(new ChangeControlTarget(Target.CategoryList), state);
                    break;
                }

                case 7:
                {
                    if (state.controlTarget == Target.SongList)
                        DispatchAction(new ChangeControlTarget(Target.SongSort), state);
                    break;
                }
            }
        }

        // UIActionの種類に応じて、UIStoreへのDispatchかSongSelectActionContextへの委譲を振り分ける
        private void DispatchAction(UIAction action, UIState state)
        {
            switch (action)
            {
                // ボタンが状態変化ではなく何かしらの操作を伴う場合にはActionContextにActionを委譲
                case ConfirmSong:
                    actionDispatchable.Dispatch(action);
                    break;
                default:
                    _dispachable.Dispatch(action);
                    break;
            }
        }
    }
}
