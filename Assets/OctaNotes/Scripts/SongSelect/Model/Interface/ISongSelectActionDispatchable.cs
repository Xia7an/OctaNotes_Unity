using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;

namespace OctaNotes.Scripts.SongSelect.Model.Interface
{
    // InputContextResolverがSongSelectActionContextに処理を委譲するためのインターフェイス
    public interface ISongSelectActionDispatchable
    {
        void Dispatch(UIAction action);
    }
}
