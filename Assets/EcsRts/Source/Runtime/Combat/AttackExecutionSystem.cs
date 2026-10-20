using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Combat
{
    /// <summary>
    /// 开火执行（文档 §7.4）：
    ///   冷却每帧全量 tick（廉价）→ 有 AttackTarget 且在射程内：
    ///   近战写 DamageEvent；远程写 ProjectileSpawnEvent（M1 远程暂同近战即时伤害）。
    ///   追击：目标在射程外且非 Hold → 写 MoveTarget（每 tick 刷新追击点）。
    /// TODO(M1, §7.4)：IJobEntity 实现；写事件用 SystemAPI.GetSingletonBuffer。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TargetAcquisitionSystem))]
    public partial struct AttackExecutionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.4)：见类注释。
        }
    }
}
