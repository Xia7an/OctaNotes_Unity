using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine.InputSystem;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class PlayInputLayer: ITickable, IDisposable, IPlayInputLayer
    {
        private const int LaneCount = 8;

        private TestInput _input;
        private readonly InputAction[] _laneActions = new InputAction[LaneCount];
        private readonly bool[] _wasPressed = new bool[LaneCount];

        public List<ReactiveProperty<ButtonState>> IsButtonPressing { get; } = new();
        public PlayInputLayer()
        {
            for (int i = 0; i < LaneCount; i++)
            {
                IsButtonPressing.Add(new ReactiveProperty<ButtonState>(ButtonState.Released));
            }
            _input = new TestInput();
            _input.Play.Enable();

            _laneActions[0] = _input.Play._0;
            _laneActions[1] = _input.Play._1;
            _laneActions[2] = _input.Play._2;
            _laneActions[3] = _input.Play._3;
            _laneActions[4] = _input.Play._4;
            _laneActions[5] = _input.Play._5;
            _laneActions[6] = _input.Play._6;
            _laneActions[7] = _input.Play._7;
        }
        
        public void Tick()
        {
            // Update to single-frame states based on current press and previous frame state.
            for (int i = 0; i < LaneCount; i++)
            {
                bool isPressed = _laneActions[i].IsPressed();
                bool wasPressed = _wasPressed[i];

                if (isPressed)
                {
                    IsButtonPressing[i].Value = wasPressed ? ButtonState.Pushed : ButtonState.BeginPush;
                }
                else
                {
                    IsButtonPressing[i].Value = wasPressed ? ButtonState.EndPush : ButtonState.Released;
                }

                _wasPressed[i] = isPressed;
            }
        }

        public void Dispose()
        {
            _input.Play.Disable();
            _input?.Dispose();
        }

    }
}
