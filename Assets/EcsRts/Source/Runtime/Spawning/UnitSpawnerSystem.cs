using EcsRts.Combat;
using EcsRts.Core;
using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Spawning
{
    /// <summary>
    /// 刷兵（文档 §7.6）：唯一允许实例化单位的系统。
    /// TODO(M1, §7.6)：
    ///  - 来源 A：InitialSpawnRequest（烘焙）——每帧最多发 SpawnBudgetPerFrame 条，Count 递减；
    ///  - 来源 B：SpawnRequestEvent（生产/AI/测试）——DelaySeconds > 0 的延迟条目存回请求位；
    ///  - 上限：场上 UnitTag 数 + 待生成数 ≥ RtsSettings.UnitCap 时丢弃并 Log 节流告警；
    ///  - UnitType 查 UnitPrefabLibrary 单例缓冲得 prefab（无库则 LogError 一次）；
    ///  - ECB.Instantiate + SetComponent：FactionId、UpdateSliceIndex（系统字段计数器 % Slices 均分）、
    ///    AttackCooldownTimer=0；生成点带小随机抖动避免完全重叠；
    ///  - 全部经 BeginSimulationEntityCommandBufferSystem 的 ECB，遵守结构变更预算（文档 §8.2）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DeathSystem))]
    public partial struct UnitSpawnerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<RtsSettings>())
                return;

            // TODO(M1, §7.6)：见类注释。
        }
    }
}
