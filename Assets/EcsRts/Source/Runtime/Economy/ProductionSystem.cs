using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Economy
{
    /// <summary>
    /// 生产（M2，文档 §7.7）：
    ///   队首 ProductionQueue.RemainingSeconds 递减 → 归零写 SpawnRequestEvent
    ///   （出生点 = 建筑 RallyPoint）；资源不足时队列冻结并 UI 提示。
    /// TODO(M2, §7.7)：见类注释与文档。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(HarvestSystem))]
    public partial struct ProductionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M2, §7.7)：见类注释。
        }
    }
}
