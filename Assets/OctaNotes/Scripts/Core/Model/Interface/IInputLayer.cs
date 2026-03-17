using System.Collections.Generic;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Core.Model.Interface
{
    public interface IInputLayer
    {
        List<ReactiveProperty<ButtonState>> IsButtonPressing { get; }
    }
}
