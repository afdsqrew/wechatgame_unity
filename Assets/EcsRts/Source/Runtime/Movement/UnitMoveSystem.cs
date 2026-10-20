using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace EcsRts.Movement
{
    /// <summary>
    /// 位置积分（文档 §7.3）：
    ///   pos += SteeringVelocity × dt；落格被阻挡则沿墙滑动（保留切向分量）；
    ///   写 LocalTransform.Position，朝向平滑转向速度方向。
    /// 顺序约束：必须 UpdateBefore(TransformSystemGroup)——LocalToWorld 由内建组推导（文档 §6）。
    /// TODO(M1, §7.3)：IJobEntity 全帧率处理（纯数学，1w 单位预算 ≤0.5ms）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SteeringSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct UnitMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.3)：见类注释。
        }
    }
}
