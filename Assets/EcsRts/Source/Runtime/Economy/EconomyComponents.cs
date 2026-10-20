using EcsRts.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Economy
{
    /// <summary>阵营资源库存（M2：每阵营一个实体挂 FactionId + ResourceStock，由生产系统首帧懒创建）。</summary>
    public struct ResourceStock : IComponentData
    {
        public int Gold;
        public int Lumber;
    }

    /// <summary>资源节点（中立实体，Faction = Neutral；存量耗尽即销毁）。</summary>
    public struct ResourceNode : IComponentData
    {
        public int Remaining;
        public float HarvestPerSecond;
    }

    /// <summary>采集者状态机（Worker 专用）。State：0=去节点 1=采集中 2=回存放点 3=卸货中。</summary>
    public struct Harvester : IComponentData
    {
        public byte State;
        public int Carried;
        public int Capacity;
        public Entity TargetNode;
    }

    /// <summary>生产队列（Buffer，挂在 BuildingTag 实体上；队首计时，完成即出兵）。</summary>
    public struct ProductionQueue : IBufferElementData
    {
        public UnitType UnitType;
        public float RemainingSeconds;
    }

    /// <summary>集结点（建筑实体；新单位出生后以 Move 指令走向此点，文档 §7.7）。</summary>
    public struct RallyPoint : IComponentData { public float3 Position; }
}
