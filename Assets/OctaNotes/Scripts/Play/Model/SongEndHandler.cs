using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Model
{
    public class SongEndHandler : ISongEndHandler
    {
        public ReactiveProperty<SongEndState> OnSongEnd { get; }
    }
}
