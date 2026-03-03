using R3;

namespace DefaultNamespace.Interface
{
    public interface ISongCategory
    {
        ReactiveProperty<Category> Category { get; }
    }
}
