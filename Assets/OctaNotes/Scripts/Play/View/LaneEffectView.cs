using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
[RequireComponent(typeof(MeshRenderer))]
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
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;

            if (_laneViewModel == null)
            {
                Debug.LogWarning($"[{nameof(LaneEffectView)}] ILaneViewModel is not injected on {gameObject.name}.", this);
                return;
            }

            _laneViewModel.ButtonState.Subscribe(buttonState =>
            {
                if (buttonState == ButtonState.BeginPush)
                {
                    ToggleOnEffect(_laneViewModel.CurrentJudge.Value);
                }
                else  if (buttonState == ButtonState.EndPush)
                {
                    ToggleOffEffect();
                }
            }).AddTo(this);
        }
        private void ToggleOnEffect(Judge judge)
        {
            material.SetFloat("_Brighten", 1f);
        }
        private void ToggleOffEffect()
        {
            material.SetFloat("_Brighten", 0f);
        }
        
    }
}
