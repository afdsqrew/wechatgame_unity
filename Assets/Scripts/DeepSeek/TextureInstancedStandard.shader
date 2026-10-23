Shader "Custom/TextureInstancedStandard"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _InstanceTex ("Instance Matrices", 2D) = "black" {}
        _InstanceTexWidth ("Instance Tex Width", Float) = 256
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0   // WebGL2 对应 GLSL ES 3.0
            // 声明实例化支持：材质的 enableInstancing 才能生效，
            // DrawMeshInstancedProcedural 走实例化绘制路径时不被拒绝（矩阵仍按 SV_InstanceID 从 _InstanceTex 取）
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _InstanceTex;
            float _InstanceTexWidth;

            // 从实例纹理重建 object-to-world 矩阵
            float4x4 LoadInstanceMatrix(uint instanceID)
            {
                float texWidth = _InstanceTexWidth;

                // 每个实例占 4 个连续像素
                uint basePixel = instanceID * 4u;
                uint pixelY = basePixel / (uint)texWidth;
                uint pixelX = basePixel % (uint)texWidth;

                // 采样 4 个像素，对应矩阵的 4 列
                float2 uv0 = (float2(pixelX + 0.5, pixelY + 0.5)) / texWidth;
                // 注意：Unity 纹理坐标原点在左下，像素行号需要翻转
                float2 uv0Flipped = float2(uv0.x, 1.0 - uv0.y);

                float4 c0 = tex2Dlod(_InstanceTex, float4(uv0Flipped, 0, 0));
                float4 c1 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(1.0/texWidth, 0), 0, 0));
                float4 c2 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(2.0/texWidth, 0), 0, 0));
                float4 c3 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(3.0/texWidth, 0), 0, 0));

                return float4x4(c0, c1, c2, c3);
            }

            Varyings vert(Attributes input, uint instanceID : SV_InstanceID)
            {
                Varyings output;

                float4x4 objectToWorld = LoadInstanceMatrix(instanceID);
                float3 positionWS = mul(objectToWorld, float4(input.positionOS.xyz, 1.0)).xyz;
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return tex2D(_MainTex, input.uv);
            }
            ENDHLSL
        }
    }
}