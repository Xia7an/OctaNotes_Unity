using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;

namespace OctaNotes.Scripts.SongSelect.Model.Actions
{
    public record SelectOption(Direction Direction) : UIAction;
}
