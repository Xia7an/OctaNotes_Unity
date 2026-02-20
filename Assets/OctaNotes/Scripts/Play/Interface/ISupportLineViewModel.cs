using System;
using System.Drawing;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ISupportLineViewModel
    {
        ReactiveProperty<double> PosZ { get; }

        event Action OnJudged;
        
        void SetInitialPosZ(double posZ);
        void SetGuids(Guid[] guids);
    }
}
