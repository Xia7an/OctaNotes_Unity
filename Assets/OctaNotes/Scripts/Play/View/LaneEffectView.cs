using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
[RequireComponent(typeof(AudioSource), typeof(MeshRenderer))]
    public class LaneEffectView : MonoBehaviour, ILaneView
    {
        private ILaneViewModel _laneViewModel;

        [Inject]
        public void Construct([InjectOptional] ILaneViewModel laneViewModel = null)
        {
            if (_laneViewModel == null && laneViewModel != null)
            {
                _laneViewModel = laneViewModel;
            }
        }

        private Material material;
        private AudioSource source;
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            source = GetComponent<AudioSource>();

            if (_laneViewModel == null)
            {
                Debug.LogWarning($"[{nameof(LaneEffectView)}] ILaneViewModel is not injected on {gameObject.name}.", this);
                return;
            }

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
