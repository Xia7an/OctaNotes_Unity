using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Interface
{
    public interface IHudViewModel
    {
        ReactiveProperty<HudParam> Param { get; }
    }
}
