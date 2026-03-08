using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;

namespace OctaNotes.Scripts.SongSelect.Model.Actions
{
    public record SelectDifficulty(Direction direction) : UIAction;
}
