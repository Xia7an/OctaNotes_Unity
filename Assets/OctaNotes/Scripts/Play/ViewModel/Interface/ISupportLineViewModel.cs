using System;
using R3;

namespace OctaNotes.Scripts.Play.ViewModel.Interface
{
    public interface ISupportLineViewModel
    {
        ReactiveProperty<double> PosZ { get; }
        void SetInitialPosZ(double posZ);
        void SetGuids(Guid[] guids);
        event Action OnJudged;
    }
}
