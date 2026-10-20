using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public static class RuntimeGoToEntityConverter
{
    /// <summary>
    /// 递归转换GameObject整棵层级，返回根Entity（Entity Prefab模板，带Prefab标签+LinkedEntityGroup）
    /// 等价旧GameObjectConversionUtility.ConvertGameObjectHierarchy
    /// </summary>
    public static Entity ConvertHierarchy(GameObject rootGo, World dstWorld)
    {
        var em = dstWorld.EntityManager;
        var rootEntity = em.CreateEntity();

        var linkedBuffer = em.AddBuffer<LinkedEntityGroup>(rootEntity);
        linkedBuffer.Add(rootEntity);

        // 递归子物体
        RecursiveConvert(rootGo.transform, rootEntity, em);

        // 标记为Entity Prefab模板，用于EntityManager.Instantiate
        em.AddComponent<Prefab>(rootEntity);

        em.AddComponent<LocalTransform>(rootEntity);
        
        em.AddComponent<LocalToWorld>(rootEntity);

        return rootEntity;
    }

    static void RecursiveConvert(Transform tf, Entity rootPrefabEntity, EntityManager em)
    {
        // 处理当前GO的MeshRenderer+MeshFilter => RenderMesh
        var mf = tf.GetComponent<MeshFilter>();
        var mr = tf.GetComponent<MeshRenderer>();
        if (mf != null && mr != null && mf.sharedMesh != null)
        {
            var currentEntity = em.CreateEntity();
            
            em.AddComponent<LocalToWorld>(currentEntity);
            em.AddComponent<RenderBounds>(currentEntity);
            // CreateEntity/AddComponent 均为结构性变更，会令之前获取的 DynamicBuffer 句柄失效，
            // 因此必须在每次结构性变更之后重新 GetBuffer 再 Add
            em.GetBuffer<LinkedEntityGroup>(rootPrefabEntity).Add(currentEntity);
            
        }

        // 递归子节点
        foreach (Transform child in tf)
        {
            RecursiveConvert(child, rootPrefabEntity, em);
        }
    }
}
