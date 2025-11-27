using UnityEngine;

namespace ArcadeControllerTest.Scripts
{
    // アケコンのボタンを押したら光る部分のView
    public class ButtonView: MonoBehaviour
    {
        [SerializeField]
        private Renderer buttonLightRenderer;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        // ボタンのライトの色を設定する
        public void SetButtonLightColor(Color color)
        {
            buttonLightRenderer.material.SetColor(EmissionColor, color);
        }
    }
}
