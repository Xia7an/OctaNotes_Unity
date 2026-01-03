using System;
using OctaNotes.Scripts.Play.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class NoteColor : MonoBehaviour
    {
        [Inject] private INoteViewModel _noteViewModel;
        [SerializeField] private MeshRenderer noteRenderer;

        private void Start()
        {
            _noteViewModel.Color.Subscribe(SetColor).AddTo(this);
        }

        private void SetColor(Color color)
        {
            noteRenderer.material.color = color;
        }
    }
}
