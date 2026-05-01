using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    // 楽曲終了時の演出イベントを提供する
    public interface ISongEndDetector
    {
        ReactiveProperty<SongEndState> OnSongEnd { get; }
    }
}
