using System;
using OctaNotes.Scripts.Play.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class LaneEffectView : MonoBehaviour
    {
        [Inject] private readonly IPlayInputLayer _playInputLayer;
        [SerializeField] private int laneIndex = 0;

        private Material material;
        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            _playInputLayer.IsButtonPressing[laneIndex].Subscribe(isPressing =>
            {
                if (isPressing)
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
        }
        private void ToggleOffEffect()
        {
            material.SetFloat("_Brighten", 0f);
        }
        
    }
}
