using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Core
{
    /// <summary>阵营。Neutral 不参与索敌（资源节点等中立物用）。</summary>
    public enum Faction : byte { Neutral = 0, Player = 1, Enemy = 2 }

    /// <summary>单位指令类型。语义见 Docs/RTS-Architecture.md §7.5。</summary>
    public enum OrderType : byte { None = 0, Move, AttackMove, AttackTarget, Stop, Hold }

    /// <summary>性能分级，驱动 RtsSettings 各项预算。参数表见文档 §9。</summary>
    public enum PerformanceTier : byte { Low = 0, Medium = 1, High = 2 }

    /// <summary>单位种类。新单位在此扩展，并在 UnitPrefabLibraryAuthoring 中登记 prefab。</summary>
    public enum UnitType : byte { Soldier = 0, Archer = 1, Worker = 2 }

    public struct UnitTag : IComponentData { }

    public struct BuildingTag : IComponentData { }

    public struct ProjectileTag : IComponentData { }

    /// <summary>单位阵营。索敌按“阵营不同且双方均非 Neutral”判定。</summary>
    public struct FactionId : IComponentData { public Faction Value; }

    /// <summary>
    /// 分帧切片索引（文档 §4.2）：昂贵逻辑每帧只处理 UpdateSliceIndex == 当前切片的单位；
    /// 全轮 = Slices × Interval。由 UnitSpawnerSystem 生成时按计数器均匀分配。
    /// </summary>
    public struct UpdateSliceIndex : IComponentData { public byte Value; }

    /// <summary>
    /// 相机世界状态快照：RtsCameraRig（LateUpdate）写入，供 Burst 系统做纯数学屏幕投影/地面射线。
    /// 消费者容忍 1 帧滞后（文档 §2）。
    /// </summary>
    public struct RtsCameraState : IComponentData
    {
        public float3 Position;
        public float3 Forward;
        public float3 Up;
        public float3 Right;
        public float FovYRadians; // 透视垂直 FOV（弧度）
        public float OrthoSize;   // IsOrtho=1 时有效（半高）
        public float Aspect;      // width / height
        public byte IsOrtho;
    }

    /// <summary>
    /// HUD/统计只读快照：StatsOverlaySystem（IJobEntity 计数）写，HudController 读。
    /// FPS 由 HUD 侧自算（Time API 非 Burst 兼容，故不入快照）。
    /// </summary>
    public struct UiSnapshot : IComponentData
    {
        public int PlayerUnitCount;
        public int EnemyUnitCount;
        public int SelectedCount;
        public int ProjectileCount;
    }

    /// <summary>
    /// 地图与网格常量（文档 §7.1/§7.2）。改动需同步：
    /// PathfindingSettings 兜底值、SpatialGridSystem 分配、ObstacleGridSystem/FlowFieldBuildSystem 分配。
    /// </summary>
    public static class RtsMapDefaults
    {
        public const float MapCellSize = 1f;      // 寻路格宽（1m）
        public const int MapDim = 256;            // 寻路格 256×256（覆盖 256m × 256m）
        public static readonly float3 MapOrigin = new float3(-128f, 0f, -128f);

        public const float SpatialCellSize = 2f;  // 空间网格格宽（2m）
        public const int SpatialDim = 128;        // 空间网格 128×128（同一地图范围）
    }
}
