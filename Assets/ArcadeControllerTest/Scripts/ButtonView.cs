using System;
using ArcadeControllerTest.Inputs;
using R3;
using UnityEngine;

namespace ArcadeControllerTest.Scripts
{
    // アケコンのボタンを押したら光る部分のView
    public class ButtonView: MonoBehaviour
    {
        [SerializeField]
        private Renderer buttonLightRenderer;
        
        private Play _playInput;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            _playInput = new Play();
            _playInput.Enable();
        }

        private void Start()
        {
            
        }

        private void OnDestroy()
        {
            _playInput.Disable();
            _playInput.Dispose();
        }

        // ボタンのライトの色を設定する
        public void SetButtonLightColor(Color color)
        {
            buttonLightRenderer.material.SetColor(EmissionColor, color);
        }
    }
}
