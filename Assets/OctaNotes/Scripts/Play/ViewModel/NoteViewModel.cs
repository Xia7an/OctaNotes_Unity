using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class NoteViewModel:MonoBehaviour, INoteViewModel
    {
        [Inject] private readonly PlaySettingsSO _playSettingsSO;
        public double PosZ { get; private set; } = new ();
        
        private double _initialPosZ = 0;
        public ReactiveProperty<Color> Color { get; } = new ReactiveProperty<Color>();
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }
        private void Update()
        {
            PosZ = -Time.time * _playSettingsSO.noteSpeed + _initialPosZ;
        }
    }
}
