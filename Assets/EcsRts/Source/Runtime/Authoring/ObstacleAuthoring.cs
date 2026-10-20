using EcsRts.Movement;
using Unity.Entities;
using UnityEngine;

namespace EcsRts.Authoring
{
    /// <summary>
    /// 静态障碍（文档 §7.2）：挂 SubScene 内的障碍物 GameObject；AABB 阻挡寻路网格与流场。
    /// </summary>
    [DisallowMultipleComponent]
    public class ObstacleAuthoring : MonoBehaviour
    {
        public Vector3 size = new Vector3(4f, 2f, 4f);
    }

    public class ObstacleBaker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new ObstacleSource
            {
                Center = authoring.transform.position,
                Size = authoring.size,
            });
        }
    }
}
