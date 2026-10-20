using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Orders
{
    /// <summary>
    /// 指令状态机（文档 §7.5）：维护 UnitOrder 生命周期。
    /// TODO(M1, §7.5)：
    ///  - Move/AttackMove：与 MoveTarget 距离 ≤ ArriveRadius 时结束（UnitOrder = None，清 MoveTarget）；
    ///  - AttackTarget：目标实体无效或已无 Health → 退化为 Stop（OrderType.None）；
    ///  - Hold：锁移动指令；Stop：清一切在途状态；
    ///  - 新指令由 OrderIssueSystem 直接覆盖，本系统不产生新指令。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(OrderIssueSystem))]
    public partial struct OrderStatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.5)：见类注释。
        }
    }
}
