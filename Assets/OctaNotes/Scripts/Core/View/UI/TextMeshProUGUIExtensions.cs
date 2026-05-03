using DG.Tweening;
using TMPro;
using UnityEngine;

namespace System.Runtime.CompilerServices.Core.View.UI
{
    // =========================================================
    // 純粋な拡張メソッド群 (別のスクリプトファイルに分けてもOKです)
    // =========================================================
    public static class TextMeshProUGUIExtensions
    {
        /// <summary>
        /// TextMeshProのGradientのアルファ値を即座に設定する
        /// </summary>
        public static void SetGradientAlpha(this TextMeshProUGUI target, float alpha)
        {
            VertexGradient vg = target.colorGradient;
            vg.topLeft.a = alpha;
            vg.topRight.a = alpha;
            vg.bottomLeft.a = alpha;
            vg.bottomRight.a = alpha;
            target.colorGradient = vg;
        }

        /// <summary>
        /// Vertex Colorのグラデーションを設定する関数
        /// horizontal = falseの場合、color1が上、color2が下
        /// horizontal = trueの場合、color1が左、color2が右
        /// </summary>
        /// <param name="target"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="horizontal"></param>
        public static void SetGradientColor(this TextMeshProUGUI target, Color color1, Color color2, bool horizontal = false)
        {
            VertexGradient vg = target.colorGradient;
            if(horizontal)
            {
                vg.topLeft =  color1;
                vg.bottomLeft = color1;
                vg.bottomRight = color2;
                vg.topRight = color2;
            }
            else
            {
                vg.topLeft = color1;
                vg.topRight = color1;
                vg.bottomLeft = color2;
                vg.bottomRight = color2;
            }
            target.colorGradient = vg;
        }


        public static void SetGradientColor(this TextMeshProUGUI target, Color topLeft, Color topRight,
            Color bottomRight, Color bottomLeft)
        {
            VertexGradient vg = target.colorGradient;
            vg.topLeft = topLeft;
            vg.topRight = topRight;
            vg.bottomLeft = bottomLeft;
            vg.bottomRight = bottomRight;
            target.colorGradient = vg;
        }
        
        

        /// <summary>
        /// TextMeshProのGradientのアルファ値をTweenさせる (DOTween標準のDOFadeと同等の使用感)
        /// </summary>
        public static Tween DOFadeGradient(this TextMeshProUGUI target, float endValue, float duration)
        {
            return DOTween.To(
                getter: () => target.colorGradient.topLeft.a,
                setter: alpha =>
                {
                    VertexGradient vg = target.colorGradient;
                    vg.topLeft.a = alpha;
                    vg.topRight.a = alpha;
                    vg.bottomLeft.a = alpha;
                    vg.bottomRight.a = alpha;
                    target.colorGradient = vg;
                },
                endValue: endValue,
                duration: duration
            ).SetTarget(target); // Tweenのターゲットを明示してKillしやすくしておく
        }
    }
}
