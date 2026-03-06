using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // ボタン入力から操作の意味に変換するクラス
    public class InputContextResolver : IInputContextResolver 
    {
        private IDispachable _dispachable;
        private IUIState _state;

        public InputContextResolver(IDispachable dispachable)
        {
            _dispachable = dispachable;
        }
        
        public void OnPhysicalButtonPressed(int buttonIdx)
        {
            UIAction action = new DoNothing();
            var state = _state.State.Value;
            switch (buttonIdx)
            {
                case 0:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Down),
                        Target.CategoryList => new SelectCategory(Direction.Down),
                        Target.GameOptions => new SelectOption(Direction.Down)
                    };
                    break;
                
                case 1:
                    action = state.controlTarget switch
                    {
                        Target.SongList => new SelectSong(Direction.Up),
                        Target.CategoryList => new SelectCategory(Direction.Up),
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
                        Target.SongList => new SelectSong(Direction.Down),
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
