using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class GameObjectConversionUtility
{
    /// <summary>
    /// 运行时把 GameObject（Prefab）整棵层级转换为 Entity Prefab 模板，返回根 Entity。
    /// - 根和所有子实体都打 Prefab 标签（对普通查询不可见），并登记进根的 LinkedEntityGroup，
    ///   因此 EntityManager / EntityCommandBuffer 的 Instantiate 会把整棵层级一起克隆；
    /// - 每个层级节点对应一个实体，子节点用 Parent + LocalTransform 保持层级（父动子随）；
    /// - 带 MeshRenderer + MeshFilter 的节点挂 TextureInstanceRenderComponent（Mesh/Material 引用），
    ///   由 DeepSeek 的 TextureInstanceRenderSystem 把世界矩阵编码进纹理后 DrawMeshInstancedProcedural 绘制，
    ///   不经过 Entities Graphics；
    /// - 这是运行时转换（配合 Addressables 动态加载的 Prefab），与 SubScene 烘焙是两条路。
    /// </summary>
    public static Entity ConvertGameObjectHierarchy(
        GameObject prefab, World settings)
    {
        if (prefab == null)
            throw new System.ArgumentNullException(
                nameof(prefab));

        var em = settings.EntityManager;

        // 根实体 = 层级锚点：Prefab 标签 + 局部变换 + LinkedEntityGroup（整组克隆的依据）
        var root = em.CreateEntity(typeof(Prefab),
            typeof(LocalTransform), typeof(LocalToWorld));
        em.SetComponentData(root,
            ToLocalTransform(prefab.transform));

        em.AddBuffer<LinkedEntityGroup>(root).Add(root);

        ConvertNode(prefab.transform, root, root, em);

        return root;
    }

    // 递归转换：nodeEntity 表示 node 这一层的实体，node 的每个子物体各生成一个新实体
    static void ConvertNode(Transform node,
        Entity nodeEntity, Entity root, EntityManager em)
    {
        var mf = node.GetComponent<MeshFilter>();
        var mr = node.GetComponent<MeshRenderer>();
        if (mf != null && mr != null &&
            mf.sharedMesh != null)
            AddRenderComponents(nodeEntity, mr, mf, em);

        foreach (Transform child in node)
        {
            // 1.4.x 变换系统按 Parent + LocalTransform 链递归计算 LocalToWorld（没有 LocalToParent 组件），
            // 父实体上的 Child 缓冲由 ParentSystem 在运行时自动维护，这里不需要手动添加
            var childEntity = em.CreateEntity(
                typeof(Prefab), typeof(Parent),
                typeof(LocalTransform),
                typeof(LocalToWorld));

            em.SetComponentData(childEntity,
                new Parent { Value = nodeEntity });
            em.SetComponentData(childEntity,
                ToLocalTransform(child));

            ConvertNode(child, childEntity, root, em);

            // CreateEntity/SetComponentData 均为结构性变更，会令之前获取的 DynamicBuffer 句柄失效，
            // 因此每次写入 LinkedEntityGroup 前都重新 GetBuffer
            em.GetBuffer<LinkedEntityGroup>(root)
                .Add(childEntity);
        }
    }

    // 不使用 RenderMeshUtility.AddComponents（Entities Graphics 渲染路径），改走 DeepSeek 纹理实例化机制：
    // 这里只挂 Mesh/Material 引用组件，渲染由 TextureInstanceRenderSystem 收集 LocalToWorld 世界矩阵、
    // 经 InstanceTextureEncoder 编码进纹理后 DrawMeshInstancedProcedural 完成；
    // 剔除用 Draw 调用传入的包围盒，不需要 Entities Graphics 的 RenderBounds
    static void AddRenderComponents(Entity entity,
        MeshRenderer mr, MeshFilter mf, EntityManager em)
    {
        // 多材质 Renderer 只取第一个材质，与原 RenderMeshArray 索引 0 的行为一致
        em.AddComponentObject(entity,
            new TextureInstanceRenderComponent
            {
                Mesh = mf.sharedMesh,
                Material = mr.sharedMaterial,
            });
    }

    // LocalTransform 只支持统一缩放，这里取 x 分量（与 GameObjectToEntityRuntimeConverter 的做法一致）
    static LocalTransform ToLocalTransform(Transform t)
    {
        return LocalTransform.FromPositionRotationScale(
            t.localPosition, t.localRotation,
            t.localScale.x);
    }
}
