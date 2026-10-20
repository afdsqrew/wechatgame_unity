Shader "Custom/TextureInstancedUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InstanceTex ("InstanceDataTex", 2D) = "black" {}
        _TexSize ("TexSize", Float) = 256
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _InstanceTex;
            float _TexSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint id : SV_InstanceID;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // 4x4矩阵存4个RGBAHalf，一行一个texel
            float4x4 LoadMatrix(uint instanceID)
            {
                float invSize = 1.0 / _TexSize;
                uint x = instanceID % (uint)_TexSize;
                uint y = instanceID / (uint)_TexSize;
                float2 uv0 = (float2(x + 0.5f, y + 0.5f) + float2(0,0)) * invSize;
                float2 uv1 = (float2(x + 0.5f, y + 0.5f) + float2(1,0)) * invSize;
                float2 uv2 = (float2(x + 0.5f, y + 0.5f) + float2(2,0)) * invSize;
                float2 uv3 = (float2(x + 0.5f, y + 0.5f) + float2(3,0)) * invSize;
                float4 r0 = tex2Dlod(_InstanceTex, float4(uv0,0,0));
                float4 r1 = tex2Dlod(_InstanceTex, float4(uv1,0,0));
                float4 r2 = tex2Dlod(_InstanceTex, float4(uv2,0,0));
                float4 r3 = tex2Dlod(_InstanceTex, float4(uv3,0,0));
                return float4x4(r0, r1, r2, r3);
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 world = LoadMatrix(v.id);
                float4 worldPos = mul(world, v.vertex);
                o.pos = TransformWorldToHClip(worldPos.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
}
