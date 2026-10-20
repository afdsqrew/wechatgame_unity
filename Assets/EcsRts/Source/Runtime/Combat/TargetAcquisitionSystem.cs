using EcsRts.Core;
using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Combat
{
    /// <summary>
    /// 索敌（切片制，文档 §4.2/§7.4）：每帧只处理一个切片的 UpdateSliceIndex 单位。
    ///   当前切片 = (int)(SystemAPI.Time.ElapsedTime / RtsSettings.TargetingIntervalSeconds)
    ///             % RtsSettings.TargetingSlices
    ///   （接入帧同步时改为系统内切片计数器自增，文档 §4.5。）
    /// TODO(M1, §7.4)：
    ///  - IJobEntity 过滤 UpdateSliceIndex == 当前切片 且无有效 AttackTarget 的单位；
    ///  - SpatialGridState 半径 AggroRange 查询 → 阵营敌对（双方非 Neutral 且不同）
    ///    且目标有 Health；距离最近者胜；
    ///  - 黏性：旧目标仍在仇恨半径内存活则不换目标（减少抖动与开销）；
    ///  - 有 MoveTarget 的 AttackMove 单位也参与（途中接敌）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EcsRts.Spatial.SpatialGridSystem))]
    public partial struct TargetAcquisitionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<RtsSettings>())
                return;

            // TODO(M1, §7.4)：见类注释。
        }
    }
}
