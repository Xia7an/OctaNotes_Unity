using System;
using System.Drawing;
using R3;

namespace OctaNotes.Scripts.Play.Interface
{
    public interface ISupportLineViewModel
    {
        double PosZ { get; }
        void SetInitialPosZ(double posZ);
        void SetGuids(Guid[] guids);
    }
}
