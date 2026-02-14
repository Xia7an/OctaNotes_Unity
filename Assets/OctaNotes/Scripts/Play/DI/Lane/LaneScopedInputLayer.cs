using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneScopedInputLayer : IPlayInputLayer, ILaneInputStateWriter
    {
        private const int LaneCount = 8;

        public List<ReactiveProperty<ButtonState>> IsButtonPressing { get; } = new();

        public LaneScopedInputLayer()
        {
            for (int i = 0; i < LaneCount; i++)
            {
                IsButtonPressing.Add(new ReactiveProperty<ButtonState>(ButtonState.Released));
            }
        }

        public void SetLaneButtonState(int laneIndex, ButtonState state)
        {
            IsButtonPressing[laneIndex].Value = state;
        }
    }
}
