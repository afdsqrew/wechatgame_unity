using UnityEngine;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Transforms;
using Unity.Rendering;

public static class GameObjectConversionSettings
{
    /// <summary>
    /// 运行时：把场景上现有的GameObject转为Entity（简易版本，只处理Transform+MeshRenderer）
    /// 仅主线程调用，不支持复杂层级、动画、多子物体，如需层级要递归写LinkedEntityGroup
    /// </summary>
    public static Entity ConvertGameObject(GameObject go, EntityManager em)
    {
        var entity = em.CreateEntity();

        // Transform
        var t = go.transform;
        em.AddComponentData(entity, LocalTransform.FromPositionRotationScale(
            t.localPosition, t.localRotation, t.localScale.x
        ));
        em.AddComponentData(entity, new LocalToWorld());

        
        var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = RenderFilterSettings.Default,  // 使用默认的过滤设置
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,  // 关闭光照探针
            };

   
        
        // 渲染部分（MeshRenderer + MeshFilter）
        var mf = go.GetComponent<MeshFilter>();
        var mr = go.GetComponent<MeshRenderer>();
        
        var renderMeshArray = new RenderMeshArray( mr.materials  , new[] { mf.mesh });

        
        if (mf != null && mr != null)
        {
            RenderMeshUtility.AddComponents(
                     entity,                // 目标实体
                     em,             // EntityManager
                    renderMeshDescription,     // 渲染网格描述
                    renderMeshArray,           // 渲染网格数组（包含 Mesh 和 Material）
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0)  // 设置材质索引和网格索引
                );
        }

        // 【可选】转换完成销毁原GameObject
        // Object.Destroy(go);

        return entity;
    }

    public static World FromWorld(World defaultGameObjectInjectionWorld, object o)
    {
        return defaultGameObjectInjectionWorld;
    }
}