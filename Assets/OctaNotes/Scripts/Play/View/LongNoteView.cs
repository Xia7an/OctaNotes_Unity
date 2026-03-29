using System;
using OctaNotes.Scripts.Play.ViewModel;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class LongNoteView : MonoBehaviour
    {
        private ILongNoteViewModel  _longNoteViewModel;
        private Material _material;
        
        [SerializeField] private LongNoteRendererRef longNoteRendererRef;

        [Inject]
        private void Construct(ILongNoteViewModel longNoteViewModel)
        {
            _longNoteViewModel = longNoteViewModel;
        }

        private void Start()
        {
            _material = longNoteRendererRef.meshRenderer.material;
            _longNoteViewModel.PosZ.Subscribe(SetPosZ).AddTo(this);
            _longNoteViewModel.IsPushed.Subscribe(SetLongBottomHide).AddTo(this);
        }
        
        protected virtual void SetPosZ(double posZ)
        {
            Vector3 pos = transform.position;
            pos.z = (float)posZ;
            transform.position = pos;
        }

        private void SetLongBottomHide(bool hide)
        {
            _material.SetFloat("_IsJudging", hide ? 1f : 0f);
        }
        
    }
}
