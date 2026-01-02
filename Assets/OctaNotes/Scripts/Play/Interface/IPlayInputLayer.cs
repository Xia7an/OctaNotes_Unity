using System.Collections.Generic;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IPlayInputLayer
    {
        List<ReactiveProperty<bool>> IsButtonPressing { get; }
    }
}
