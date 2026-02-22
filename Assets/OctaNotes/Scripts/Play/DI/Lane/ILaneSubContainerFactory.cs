using OctaNotes.Scripts.Play.View;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public interface ILaneSubContainerFactory
    {
        DiContainer CreateLaneSubContainer(int laneIndex);
        DiContainer GetLaneSubContainer(int laneIndex);
    }
}
