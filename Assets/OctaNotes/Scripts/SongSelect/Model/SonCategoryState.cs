using DefaultNamespace;
using DefaultNamespace.Interface;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class SonCategoryState : ISongCategory
    {
        public ReactiveProperty<Category> Category { get; } = new();
    }
}
