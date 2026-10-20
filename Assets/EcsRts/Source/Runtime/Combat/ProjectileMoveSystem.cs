using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Combat
{
    /// <summary>
    /// 投射物弹道（M2，文档 §7.4）：
    ///   消费 ProjectileSpawnEvent → 受 RtsSettings.ProjectileCap 约束下 ECB 实例化（ProjectileTag + Projectile 组件）；
    ///   每帧推进、近距判定命中 → 写 DamageEvent → ECB 销毁；TTL 到期直接销毁。
    /// TODO(M2, §7.4)：见类注释。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AttackExecutionSystem))]
    public partial struct ProjectileMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M2, §7.4)：见类注释。
        }
    }
}
