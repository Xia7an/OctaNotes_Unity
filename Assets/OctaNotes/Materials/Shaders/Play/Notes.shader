Shader "Custom/URP/ConstantWidthOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width (px)", Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry+10"
            "RenderType"="Opaque"
        }

        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float4 _OutlineColor;
            float  _OutlineWidth;

            Varyings vert (Attributes v)
            {
                Varyings o;

                // オブジェクト → ワールド
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS   = TransformObjectToWorldNormal(v.normalOS);

                // ワールド → クリップ
                float4 positionCS = TransformWorldToHClip(positionWS);

                // 画面解像度補正
                float2 screenSize = _ScreenParams.xy;

                // 法線をビュー空間へ
                float3 normalVS = TransformWorldToViewDir(normalWS);

                // スクリーン空間でのオフセット
                float2 offset = normalize(normalVS.xy);

                // ピクセル単位 → NDC → Clip Space
                offset *= _OutlineWidth * 2.0 / screenSize;
                offset *= positionCS.w;

                positionCS.xy += offset;

                o.positionCS = positionCS;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
