using System;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
[RequireComponent(typeof(AudioSource), typeof(MeshRenderer))]
    public class LaneEffectView : MonoBehaviour, ILaneView
    {
        private LaneViewModel _laneViewModel;

        [Inject]
        public void Construct(LaneViewModel laneViewModel)
        {
            this._laneViewModel = laneViewModel;
        }

        private Material material;
        private AudioSource source;
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            source = GetComponent<AudioSource>();
            _laneViewModel.ButtonState.Subscribe(buttonState =>
            {
                if (buttonState == ButtonState.BeginPush)
                {
                    ToggleOnEffect();
                }
                else  if (buttonState == ButtonState.EndPush)
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
