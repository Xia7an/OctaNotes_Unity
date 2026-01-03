using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class PlayInputLayer: ITickable, IDisposable, IPlayInputLayer
    {
        private TestInput _input;

        public PlayInputLayer()
        {
            for (int i = 0; i < 8; i++)
            {
                IsButtonPressing.Add(new ReactiveProperty<bool>(false));
            }
            _input = new TestInput();
            _input.Play.Enable();
        }
        
        public void Tick()
        {
            _input.Play._0.started += ctx => {IsButtonPressing[0].Value = true;};
            _input.Play._0.canceled += ctx => {IsButtonPressing[0].Value = false;};
            _input.Play._1.started += ctx => {IsButtonPressing[1].Value = true;};
            _input.Play._1.canceled += ctx => {IsButtonPressing[1].Value = false;};
            _input.Play._2.started += ctx => {IsButtonPressing[2].Value = true;};
            _input.Play._2.canceled += ctx => {IsButtonPressing[2].Value = false;};
            _input.Play._3.started += ctx => {IsButtonPressing[3].Value = true;};
            _input.Play._3.canceled += ctx => {IsButtonPressing[3].Value = false;};
            _input.Play._4.started += ctx => {IsButtonPressing[4].Value = true;};
            _input.Play._4.canceled += ctx => {IsButtonPressing[4].Value = false;};
            _input.Play._5.started += ctx => {IsButtonPressing[5].Value = true;};
            _input.Play._5.canceled += ctx => {IsButtonPressing[5].Value = false;};
            _input.Play._6.started += ctx => {IsButtonPressing[6].Value = true;};
            _input.Play._6.canceled += ctx => {IsButtonPressing[6].Value = false;};
            _input.Play._7.started += ctx => {IsButtonPressing[7].Value = true;};
            _input.Play._7.canceled += ctx => {IsButtonPressing[7].Value = false;};
        }

        public void Dispose()
        {
            _input?.Dispose();
        }

        public List<ReactiveProperty<bool>> IsButtonPressing { get; } = new List<ReactiveProperty<bool>>();
    }
}
