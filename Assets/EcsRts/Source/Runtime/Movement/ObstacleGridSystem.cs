using EcsRts.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace EcsRts.Movement
{
    /// <summary>
    /// 阻挡位图维护（文档 §7.2）：把全部 ObstacleSource 栅格化为 NativeBitArray。
    /// 重建仅在障碍集合变化时发生（M1：字段脏标记每 30 帧校验一次即可）。
    /// 静态只读视图：FlowFieldBuildSystem 与 SteeringSystem 共用（Job 拷贝句柄传入）。
    /// TODO(M1, §7.2)：
    ///  - OnUpdate：有脏标记时全量重建（AABB → 覆盖格置位）；
    ///  - 建筑落成（M2）与 ObstacleAuthoring 变更时置脏。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ObstacleGridSystem : ISystem
    {
        private NativeBitArray _blocked;

        public void OnCreate(ref SystemState state)
        {
            _blocked = new NativeBitArray(RtsMapDefaults.MapDim * RtsMapDefaults.MapDim, Allocator.Persistent);
            // TODO(M1, §7.2)：加脏标记（ObstacleSource 集合变化 / 建筑落成时置脏），首帧必建。
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_blocked.IsCreated)
                _blocked.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.2)：_dirty 时全量重建（SystemAPI.Query<RefRO<ObstacleSource>>），
            // 完成后 _dirty = false。静态视图暴露给消费系统。
        }
    }
}
