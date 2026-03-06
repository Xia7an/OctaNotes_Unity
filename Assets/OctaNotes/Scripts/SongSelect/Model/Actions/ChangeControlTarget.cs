using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;

namespace OctaNotes.Scripts.SongSelect.Model.Actions
{
    public record ChangeControlTarget(Target Target) : UIAction;
}
