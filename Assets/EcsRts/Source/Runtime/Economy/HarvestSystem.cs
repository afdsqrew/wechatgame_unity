using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Economy
{
    /// <summary>
    /// 采集回路（M2，文档 §7.7）：
    ///   Harvester 状态机：去最近节点（空间网格查询 ResourceNode）→ 采集（节点 Remaining 递减）
    ///   → 满载返回存放点 → 入库 ResourceStock → 循环；节点耗尽 → ECB 销毁 + 重选节点。
    /// TODO(M2, §7.7)：见类注释与文档。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct HarvestSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M2, §7.7)：见类注释。
        }
    }
}
