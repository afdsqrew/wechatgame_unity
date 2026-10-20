using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Combat
{
    /// <summary>
    /// 死亡处理（文档 §7.4）：Health.Current ≤ 0 →
    ///   ECB 销毁（含血条/选中环子实体，用 ECB.Playback 默认级联规则）
    ///   + 写 DeathEvent（供统计/AI/呈现消费；M1 仅统计）。
    /// 注意：本系统天然先于 EventBufferClearSystem（OrderLast，文档 §6）。
    /// TODO(M1, §7.4)：IJobEntity 扫描 + ECB。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DamageApplicationSystem))]
    public partial struct DeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.4)：见类注释。
        }
    }
}
