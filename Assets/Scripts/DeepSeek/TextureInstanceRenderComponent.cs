using Unity.Entities;
using UnityEngine;

/// <summary>
/// DeepSeek 纹理实例化渲染数据：GameObjectConversionUtility.AddRenderComponents 转换时挂到实体上，
/// 替代 Entities Graphics 的 RenderMesh 组件（不再走 RenderMeshUtility.AddComponents）。
/// TextureInstanceRenderSystem 按 (Mesh, Material) 分组收集实体的 LocalToWorld 世界矩阵，
/// 经 InstanceTextureEncoder 编码进纹理后用 DrawMeshInstancedProcedural 绘制。
/// Mesh / Material 是托管引用，因此组件必须是托管 IComponentData。
/// </summary>
public class TextureInstanceRenderComponent : IComponentData
{
    public Mesh Mesh;
    public Material Material;
}
