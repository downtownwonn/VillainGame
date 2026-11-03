Shader "VillanGame/UI/ChromaKeyVideo"
{
    Properties
    {
        [PerRendererData] _MainTex ("Video Texture", 2D) = "white" {}
        _Color ("UI Tint", Color) = (1, 1, 1, 1)
        _KeyColor ("Key Color", Color) = (0, 1, 0, 1)
        _Threshold ("Key Threshold", Range(0.01, 0.6)) = 0.22
        _Softness ("Edge Softness", Range(0.01, 0.4)) = 0.1
        _GreenDominanceStart ("Green Matte Start", Range(0.01, 0.5)) = 0.05
        _GreenDominanceEnd ("Green Matte End", Range(0.02, 0.8)) = 0.28
        _SpillReduction ("Green Spill Reduction", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _KeyColor;
            float _Threshold;
            float _Softness;
            float _GreenDominanceStart;
            float _GreenDominanceEnd;
            float _SpillReduction;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata input)
            {
                v2f output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                fixed4 sampleColor = tex2D(_MainTex, input.uv) * input.color;

                // Compare chroma (R-G, B-G), so the key remains stable across
                // darker and lighter green pixels in a compressed video.
                float2 pixelChroma = float2(sampleColor.r - sampleColor.g, sampleColor.b - sampleColor.g);
                float2 keyChroma = float2(_KeyColor.r - _KeyColor.g, _KeyColor.b - _KeyColor.g);
                float chromaDistance = distance(pixelChroma, keyChroma);
                float chromaAlpha = smoothstep(_Threshold, _Threshold + _Softness, chromaDistance);

                float greenExcess = max(0.0, sampleColor.g - max(sampleColor.r, sampleColor.b));
                float greenMatte = smoothstep(_GreenDominanceStart, _GreenDominanceEnd, greenExcess);
                // Preserve very dark line art such as the black subway handles.
                // This is luminance based, so it protects the drawn detail without
                // restoring arbitrary screen corners or green-screen pixels.
                float luminance = dot(sampleColor.rgb, float3(0.2126, 0.7152, 0.0722));
                float darkDetail = 1.0 - smoothstep(0.02, 0.18, luminance);
                float alpha = lerp(chromaAlpha * (1.0 - greenMatte), 1.0, darkDetail);
                float spill = greenMatte * _SpillReduction * (1.0 - alpha * 0.75) * (1.0 - darkDetail);
                sampleColor.g = lerp(sampleColor.g, max(sampleColor.r, sampleColor.b), spill);

                sampleColor.a *= alpha;
                return sampleColor;
            }
            ENDHLSL
        }
    }
}
