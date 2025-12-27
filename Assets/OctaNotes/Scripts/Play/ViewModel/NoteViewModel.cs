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
        public ReactiveProperty<Color> Color { get; } = new ReactiveProperty<Color>();
        public void SetPosZ(double posZ)
        {
            PosZ = posZ;
        }
        private void Update()
        {
            PosZ -= Time.deltaTime * _playSettingsSO.noteSpeed;
        }
    }
}
