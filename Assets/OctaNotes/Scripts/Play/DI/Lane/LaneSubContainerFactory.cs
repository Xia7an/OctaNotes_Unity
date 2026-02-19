using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.View;
using System.Collections.Generic;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneSubContainerFactory : ILaneSubContainerFactory
    {
        private readonly DiContainer _container;
        private readonly Dictionary<int, DiContainer> _laneContainers = new();

        public LaneSubContainerFactory(DiContainer container)
        {
            _container = container;
        }

        public DiContainer CreateLaneSubContainer(int laneIndex)
        {
            var subContainer = _container.CreateSubContainer();
            _container.BindInterfacesTo<Kernel>().FromSubContainerResolve().ByInstance(subContainer).AsCached();
            subContainer.Bind<Kernel>().AsCached();
            LaneSubContainerInstaller.Install(subContainer, laneIndex);
            subContainer.ResolveRoots();
            _laneContainers[laneIndex] = subContainer;
            return subContainer;
        }

        public DiContainer GetLaneSubContainer(int laneIndex)
        {
            if (!_laneContainers.TryGetValue(laneIndex, out var laneContainer))
            {
                throw new ZenjectException($"Lane sub-container for lane index {laneIndex} is not created.");
            }

            return laneContainer;
        }
    }
}
