using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneSubContainerFactory : ILaneSubContainerFactory
    {
        public void BindLane(DiContainer container, int laneIndex)
        {
            container.Bind(typeof(ILaneInputPort), typeof(ILaneOutputPort), typeof(ILaneViewModel))
                .FromSubContainerResolve()
                .ByMethod(subContainer => LaneSubContainerInstaller.Install(subContainer, laneIndex))
                .WithKernel()
                .AsCached();
        }
    }
}
