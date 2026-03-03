using DefaultNamespace;
using DefaultNamespace.Interface;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class SelectedDifficultyState :  ISelectedDifficulty
    {
        public ReactiveProperty<Difficulty> SelectedDifficulty { get; } = new(Difficulty.Dual);
    }
}
