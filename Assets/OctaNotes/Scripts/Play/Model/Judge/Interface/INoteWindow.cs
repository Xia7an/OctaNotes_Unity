using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface INoteWindow
    {
        ReactiveProperty<Note> CurrentNote { get; }
    }
}
