using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Graphics;

namespace EcsRts.Selection
{
    /// <summary>
    /// 应用拾取结果（文档 §7.5）：
    ///   消费 SelectionResult → 用 BeginSimulationEntityCommandBufferSystem 的 ECB
    ///   为命中单位 AddComponent&lt;Selected&gt;、为取消单位 RemoveComponent&lt;Selected&gt;。
    ///   M2 切换为 SetComponentEnabled（类型已按 enableable 声明，文档 §4.4）。
    /// TODO(M1, §7.5)：
    ///  - 保留/清空语义：Shift 追加、点空地清空；
    ///  - 选中数写入 UiSnapshot.SelectedCount（或由 StatsOverlaySystem 统计，二选一，此处只改状态）；
    ///  - 选中视觉（M1：URPMaterialPropertyBaseColor 提亮）交给 Presentation 的 SelectionRingSystem。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SelectionApplySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.5)：见类注释。
        }
    }
}
