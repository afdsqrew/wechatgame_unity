using EcsRts.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Spawning
{
    /// <summary>
    /// 烘焙初值（SpawnerAuthoring → Baker）：首帧由 UnitSpawnerSystem 分批转成 SpawnRequestEvent。
    /// 与 SpawnRequestEvent 分开，避免烘焙数据在首帧一次性进入事件缓冲造成尖峰。
    /// </summary>
    public struct InitialSpawnRequest : IComponentData
    {
        public UnitType UnitType;
        public Faction Faction;
        public int Count;          // 剩余待生成数（系统递减）
        public float3 Center;
        public float Radius;
    }

    /// <summary>生成请求（单例缓冲，文档 §4.3）：刷兵/生产/AI 写 → UnitSpawnerSystem 预算消费。</summary>
    public struct SpawnRequestEvent : IBufferElementData
    {
        public UnitType UnitType;
        public Faction Faction;
        public float3 Position;
        public float Yaw;
        public float DelaySeconds; // > 0 时由刷兵系统延迟生效
    }

    /// <summary>UnitType → 烘焙 prefab 实体映射（单例缓冲，UnitPrefabLibraryAuthoring 烘焙）。</summary>
    public struct UnitPrefabLibrary : IBufferElementData
    {
        public UnitType UnitType;
        public Entity Prefab;
    }
}
