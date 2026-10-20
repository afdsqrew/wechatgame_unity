using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace EcsRts.Combat
{
    /// <summary>
    /// 伤害应用（文档 §7.4）：同一目标同帧多伤必须先聚合再应用，避免读写竞争。
    /// TODO(M1, §7.4)：
    ///  - 遍历 DamageEvent 单例缓冲 → NativeParallelHashMap<Entity, float> 求和（TempJob）；
    ///  - IJobEntity 携带该表对 Health 一次性扣减（clamp ≥ 0）；
    ///  - 无伤害事件时直接返回（零开销快路径）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AttackExecutionSystem))]
    public partial struct DamageApplicationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.4)：见类注释。
        }
    }
}
