using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Orders
{
    /// <summary>
    /// 指令下发（文档 §7.5）：把一条 OrderIntentEvent 展开为全体选中单位的 UnitOrder + MoveTarget。
    /// TODO(M1, §7.5)：
    ///  - 阵型槽位：以 TargetPosition 为中心生成方阵槽（间距 = 2 × RtsSettings.UnitRadius），
    ///    按“单位到槽位距离”贪心就近分配（M1 单 Job 完成；M2 换分桶/匈牙利）；
    ///  - AttackMove：写 UnitOrder{AttackMove, Destination}，清 AttackTarget（途中由索敌接管）；
    ///  - Move/Stop/Hold/AttackTarget 直写对应字段；
    ///  - 只处理 Player 阵营的选中单位。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EcsRts.Selection.SelectionApplySystem))]
    public partial struct OrderIssueSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.5)：见类注释。
        }
    }
}
