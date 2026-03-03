using System.Collections.Generic;
using R3;

namespace DefaultNamespace.Interface
{
    public interface ISongList
    {
        ReactiveProperty<List<SongData>> Data { get; }
    }
}
