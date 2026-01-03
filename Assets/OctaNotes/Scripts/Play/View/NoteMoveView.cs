using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.ViewModel;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class NoteMoveView: MonoBehaviour
    {
        [SerializeField] private NoteViewModel _noteViewModel;


        public virtual void setPosZ(double posZ)
        {
            Vector3 pos = transform.position;
            pos.z = (float)posZ;
            transform.position = pos;
        }
        private void Update()
        {
            setPosZ(_noteViewModel.PosZ);
        }
        
    }
}
