using System;
using R3;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface INoteViewModel
    {
        ReactiveProperty<double> PosZ { get; }
        
        ReactiveProperty<Color> Color { get; }

        event Action OnJudged;
        
        void SetInitialPosZ(double posZ);
        
        void SetGuid(Guid guid);
        
    }
}
