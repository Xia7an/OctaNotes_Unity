using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface IPlayInputLayer
    {
        List<ReactiveProperty<ButtonState>> IsButtonPressing { get; }
    }
}
