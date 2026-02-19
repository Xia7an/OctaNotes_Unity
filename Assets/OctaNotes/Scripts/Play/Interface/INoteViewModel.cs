using System;
using R3;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface INoteViewModel
    {
        double PosZ { get; }
        
        ReactiveProperty<Color> Color { get; }
        
        void SetInitialPosZ(double posZ);
        
        void SetGuid(Guid guid);
        
    }
}
