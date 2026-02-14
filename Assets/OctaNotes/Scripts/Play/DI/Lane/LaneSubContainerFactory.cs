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

        public void BindLane(DiContainer container, int laneIndex, ILaneView[] laneViews)
        {
            container.Bind(typeof(ILaneInputPort), typeof(ILaneOutputPort), typeof(ILaneViewModel))
                .FromSubContainerResolve()
                .ByMethod(subContainer =>
                {
                    _laneContainers[laneIndex] = subContainer;
                    LaneSubContainerInstaller.Install(subContainer, laneIndex);
                    for (int laneViewIndex = 0; laneViewIndex < laneViews.Length; laneViewIndex++)
                    {
                        subContainer.Bind<ILaneView>().WithId(laneViewIndex).FromInstance(laneViews[laneViewIndex]).AsCached();
                    }
                })
                .WithKernel()
                .AsCached();
        }

        public DiContainer GetLaneSubContainer(int laneIndex)
        {
            if (!_laneContainers.TryGetValue(laneIndex, out var laneContainer))
            {
                throw new ZenjectException($"Lane sub-container for lane index {laneIndex} is not created.");
            }

            return laneContainer;
        }

        public T ResolveFromLane<T>(int laneIndex)
        {
            return GetLaneSubContainer(laneIndex).Resolve<T>();
        }

        public T ResolveFromLaneId<T>(int laneIndex, object identifier)
        {
            return GetLaneSubContainer(laneIndex).ResolveId<T>(identifier);
        }
    }
}
