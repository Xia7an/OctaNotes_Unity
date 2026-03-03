using R3;

namespace DefaultNamespace.Interface
{
    public interface ISelectedDifficulty
    {
        ReactiveProperty<Difficulty>  SelectedDifficulty { get; }
    }
}
