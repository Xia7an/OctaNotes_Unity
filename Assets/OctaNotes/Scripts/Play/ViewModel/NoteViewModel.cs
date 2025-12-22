using System;
using OctaNotes.Scripts.Play.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class NoteViewModel:MonoBehaviour, INoteViewModel
    {
        public double PosZ { get; private set; } = new ();
        public ReactiveProperty<Color> Color { get; } = new ReactiveProperty<Color>();
        public void SetPosZ(double posZ)
        {
            PosZ = posZ;
        }
        private void Update()
        {
            PosZ += 1f / Application.targetFrameRate;
        }
    }
}
