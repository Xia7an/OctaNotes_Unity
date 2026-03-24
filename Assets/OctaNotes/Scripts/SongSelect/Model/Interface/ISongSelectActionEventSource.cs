using System;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    // ViewModelがSongSelectActionContextのイベントを購読するためのインターフェイス
    public interface ISongSelectActionEventSource
    {
        event Action<UIAction> OnActionDispatched;
    }
}
