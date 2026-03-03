using System.Collections.Generic;
using DefaultNamespace.Interface;
using R3;

namespace DefaultNamespace
{
    public class SongListManager : ISongList
    {
        public ReactiveProperty<List<SongData>> Data { get; } =  new (new());
    }
}
