using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.ViewModel.Interface
{
    public interface ISongEndUIViewModel
    {
        ReactiveProperty<ClearMark> ShowClearMark { get; }
    }
}
