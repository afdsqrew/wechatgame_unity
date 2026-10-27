Shader "Custom/TextureInstancedMeshAnim"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _InstanceTex ("Instance Matrices", 2D) = "black" {}
        _InstanceTexWidth ("Instance Tex Width", Float) = 256
        _AnimTextures ("Anim Textures", 2DArray) = "" {}
        _AnimInfo ("Anim Info", Vector) = (0, 0, 0, 0)
        _AnimTimeInfo ("Anim Time Info", Vector) = (0, 0, 0, 1)
        _AnimScalar ("Anim Scalar", Vector) = (1, 1, 1, 0)
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
            // SV_VertexID + Texture2DArray：WebGL2（GLSL ES 3.0）与桌面均可用
            #pragma target 3.5
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // 复用 MeshAnimator 的烘焙采样逻辑（GetPixelOffset/ApplyMeshAnimation）
            #include "../../MeshAnimator/Shaders/MeshAnimator.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                uint instanceID : SV_InstanceID;
                uint vertexId : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _InstanceTex;
            float _InstanceTexWidth;
            float4 _AnimInfo;
            float4 _AnimTimeInfo;
            float4 _AnimScalar;

            TEXTURE2D_ARRAY(_AnimTextures);
            SAMPLER(sampler_AnimTextures);

            // 从实例纹理重建 object-to-world 矩阵（与 TextureInstancedStandard 一致）
            float4x4 LoadInstanceMatrix(uint instanceID)
            {
                float texWidth = _InstanceTexWidth;

                uint basePixel = instanceID * 4u;
                uint pixelY = basePixel / (uint)texWidth;
                uint pixelX = basePixel % (uint)texWidth;

                float2 uv0 = (float2(pixelX + 0.5, pixelY + 0.5)) / texWidth;
                float2 uv0Flipped = float2(uv0.x, 1.0 - uv0.y);

                float4 c0 = tex2Dlod(_InstanceTex, float4(uv0Flipped, 0, 0));
                float4 c1 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(1.0/texWidth, 0), 0, 0));
                float4 c2 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(2.0/texWidth, 0), 0, 0));
                float4 c3 = tex2Dlod(_InstanceTex, float4(uv0Flipped + float2(3.0/texWidth, 0), 0, 0));

                return float4x4(c0, c1, c2, c3);
            }

            // 每实例 0~1 的稳定随机相位：SV_InstanceID 在无结构变更时逐帧稳定，
            // 用它哈希出相位即可把整群动画错开，无需 CPU 侧逐实例数据
            float InstancePhase(uint instanceID)
            {
                return frac(sin(instanceID * 12.9898) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                float4x4 objectToWorld = LoadInstanceMatrix(input.instanceID);

                // 相位偏移通过平移动画时间窗实现：z/w 同减 phase*时长，窗口长度不变，
                // GetPixelOffset 内部 (_Time.y - z) / (w - z) 即按实例相位错开
                float4 timeInfo = _AnimTimeInfo;
                float duration = max(timeInfo.w - timeInfo.z, 0.001);
                float phaseShift = InstancePhase(input.instanceID) * duration;
                timeInfo.z -= phaseShift;
                timeInfo.w -= phaseShift;

                // crossfade 四个参数全零：crossfadeData.y(0) > _Time.y 恒为假，交叉淡入分支不生效
                float3 positionOS = ApplyMeshAnimation(
                    input.positionOS.xyz,
                    input.vertexId,
                    _AnimInfo,
                    timeInfo,
                    _AnimScalar,
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0),
                    _AnimTextures,
                    sampler_AnimTextures);

                float3 positionWS = mul(objectToWorld, float4(positionOS, 1.0)).xyz;
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
