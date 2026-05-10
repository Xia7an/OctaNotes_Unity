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
        private INoteViewModel _noteViewModel;

        [Inject]
        private void Construct(INoteViewModel noteViewModel)
        {
            _noteViewModel = noteViewModel;
        }

        private void Start()
        {
            _noteViewModel.PosZ.Subscribe(SetPosZ).AddTo(this);
            Observable.FromEvent(h => _noteViewModel.OnJudged += h, 
                h => _noteViewModel.OnJudged -= h)
                .Subscribe(_ => InvokeJudgedEffect())
                .AddTo(this);
        }

        protected virtual void SetPosZ(double posZ)
        {
            Vector3 pos = transform.position;
            pos.z = (float)posZ;
            transform.position = pos;
        }

        private void InvokeJudgedEffect()
        {
            gameObject.SetActive(false);
        }
    }
}
