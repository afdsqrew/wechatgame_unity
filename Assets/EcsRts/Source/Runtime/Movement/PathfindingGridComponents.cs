using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Movement
{
    /// <summary>寻路网格配置单例（文档 §7.2）。默认与 RtsMapDefaults 一致，由 RtsInitializationSystem 兜底创建。</summary>
    public struct PathfindingSettings : IComponentData
    {
        public float3 Origin;  // 世界原点（地图左下角）
        public float CellSize; // 格宽（米）
        public int2 Dims;      // 格数（x, z）
    }

    /// <summary>
    /// 障碍 AABB 源（文档 §7.2）：ObstacleAuthoring 烘焙；M2 建筑落成时也写入。
    /// ObstacleGridSystem 据此维护阻挡位图。
    /// </summary>
    public struct ObstacleSource : IComponentData
    {
        public float3 Center;
        public float3 Size; // 全尺寸（x/z 生效）
    }
}
