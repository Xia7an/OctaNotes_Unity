using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.Model.Actions
{
    public record ReloadSongList(SongData[] SongList) : UIAction;
}
