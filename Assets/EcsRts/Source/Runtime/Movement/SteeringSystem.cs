using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Movement
{
    /// <summary>
    /// 转向（文档 §7.3，全帧率纯数学）：
    ///   flowDir（流场采样）× 权重 + separation（空间网格邻域 ≤8 采样）+ arrive（半径内减速）
    ///   → 限幅 MoveSpeed → 写 SteeringVelocity。
    /// TODO(M1, §7.3)：
    ///  - IJobEntity：Query RefRW<SteeringVelocity>, RefRO<MoveTarget/MoveSpeed>（有 MoveTarget 才处理）；
    //   - 邻居采样：SpatialGridState.Cells 半径 2×UnitRadius，≤8 个即截断；
    ///  - 热点时把 separation 采样切到 UpdateSliceIndex 分帧（注释位保留，文档 §7.3）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FlowFieldBuildSystem))]
    public partial struct SteeringSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.3)：见类注释。
        }
    }
}
