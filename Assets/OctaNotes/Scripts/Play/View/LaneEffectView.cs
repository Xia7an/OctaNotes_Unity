using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
[RequireComponent(typeof(AudioSource), typeof(MeshRenderer))]
    public class LaneEffectView : MonoBehaviour
    {
        [Inject] private readonly List<ILaneViewModel> _laneViewModels;
        [SerializeField] private int laneIndex = 0;

        private Material material;
        private AudioSource source;
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            source = GetComponent<AudioSource>();
            _laneViewModels[laneIndex].ButtonState.Subscribe(buttonState =>
            {
                if (buttonState == ButtonState.BeginPush || buttonState == ButtonState.Pushed)
                {
                    ToggleOnEffect();
                }
                else
                {
                    ToggleOffEffect();
                }
            });
        }
        private void ToggleOnEffect()
        {
            material.SetFloat("_Brighten", 1f);
            source.PlayOneShot(source.clip);
        }
        private void ToggleOffEffect()
        {
            material.SetFloat("_Brighten", 0f);
        }
        
    }
}
