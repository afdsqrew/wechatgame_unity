using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Selection
{
    /// <summary>
    /// 单击拾取（文档 §7.5）：无物理引擎，走纯数学射线。
    /// TODO(M1, §7.5)：
    ///  - 由 RtsCameraState 构造射线，与地面平面 y=0 求交；
    ///  - SpatialGridState 半径查询（半径 = RtsSettings.UnitRadius × 容差倍数），
    ///    候选中 Selectable.Priority 高者优先、距离近者胜；
    ///  - 命中写 SelectionResult；未命中 = 点空地清选。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RtsInputSystem))]
    public partial struct ClickSelectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.5)：见类注释。
        }
    }
}
