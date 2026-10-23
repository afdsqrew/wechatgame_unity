using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class TextureInstanceRenderSystem : SystemBase
{
    // DeepSeek 机制的实例化 Shader：顶点阶段按 SV_InstanceID 从 _InstanceTex 取回实例矩阵
    private const string InstancedShaderName = "Custom/TextureInstancedStandard";
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");

    // 每个 (Mesh, Material) 组合一组：组内全部实例的世界矩阵编码进同一张纹理，
    // 用一次（或按 MaxPerDraw 拆分的多次）DrawMeshInstancedProcedural 绘制
    private class RenderGroup
    {
        public TextureInstancedRenderer Renderer;
        public Texture2D InstanceTex;
        public readonly List<float4x4> Matrices = new List<float4x4>();
    }

    private readonly Dictionary<(Mesh mesh, Material material), RenderGroup> _groups =
        new Dictionary<(Mesh, Material), RenderGroup>();

    private Shader _instancedShader;

    protected override void OnCreate()
    {
        // Shader.Find 在 WebGL 包里只对“被资源引用或加入 Always Included Shaders”的 Shader 生效
        _instancedShader = Shader.Find(InstancedShaderName);
        if (_instancedShader == null)
            Debug.LogError($"[TextureInstanceRenderSystem] 找不到 Shader '{InstancedShaderName}'，" +
                "请把它加入 Project Settings > Graphics > Always Included Shaders，否则实例化材质无法创建。");
    }

    protected override void OnUpdate()
    {
        if (_instancedShader == null)
            return;

        foreach (var group in _groups.Values)
            group.Matrices.Clear();

        // 按 (Mesh, Material) 分组收集世界矩阵；Prefab 模板带 Prefab 标签，查询自动排除
        foreach (var (localToWorld, render) in
                 SystemAPI.Query<RefRO<LocalToWorld>, TextureInstanceRenderComponent>())
        {
            if (render.Mesh == null || render.Material == null)
                continue;

            var key = (render.Mesh, render.Material);
            if (!_groups.TryGetValue(key, out var group))
            {
                group = new RenderGroup
                {
                    Renderer = new TextureInstancedRenderer(
                        CreateGroupMaterial(render.Material), render.Mesh),
                };
                _groups.Add(key, group);
            }
            group.Matrices.Add(localToWorld.ValueRO.Value);
        }

        var camera = Camera.main;
        foreach (var group in _groups.Values)
        {
            if (group.Matrices.Count == 0)
                continue;

            // 矩阵编码进纹理，Shader 端按 SV_InstanceID 逐实例取回，单次绘制出全部实例
            using (var matrices = new NativeArray<float4x4>(group.Matrices.ToArray(), Allocator.TempJob))
            {
                group.InstanceTex = InstanceTextureEncoder.EncodeMatrices(matrices, group.InstanceTex);
                group.Renderer.UpdateInstances(group.InstanceTex, group.Matrices.Count);
                group.Renderer.Draw(camera);
            }
        }
    }

    protected override void OnDestroy()
    {
        foreach (var group in _groups.Values)
            group.Renderer.Dispose(); // Dispose 内会销毁持有的实例纹理
        _groups.Clear();
    }

    // 每组要写入各自的 _InstanceTex，不能直接用共享材质（避免污染原始资源）；
    // 材质 Shader 不支持 procedural instancing 时，换用 DeepSeek 实例化 Shader 并拷贝主贴图
    Material CreateGroupMaterial(Material source)
    {
        Material material = source.shader == _instancedShader
            ? new Material(source)
            : CreateAdaptedMaterial(source);
        material.enableInstancing = true;
        return material;
    }

    Material CreateAdaptedMaterial(Material source)
    {
        var material = new Material(_instancedShader);
        // 原材质是 URP 材质时主贴图存放在 _BaseMap，Built-in 材质则是 _MainTex
        var mainTex = source.HasProperty(MainTexId) ? source.GetTexture(MainTexId)
            : source.HasProperty(BaseMapId) ? source.GetTexture(BaseMapId)
            : null;
        if (mainTex != null)
            material.SetTexture(MainTexId, mainTex);
        return material;
    }
}
