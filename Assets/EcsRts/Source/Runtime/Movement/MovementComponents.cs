using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Movement
{
    /// <summary>最大移动速度（m/s）。</summary>
    public struct MoveSpeed : IComponentData { public float Value; }

    /// <summary>当前移动目的地。ArriveRadius 内视为到达（由 OrderStatusSystem/战斗追击写入）。</summary>
    public struct MoveTarget : IComponentData
    {
        public float3 Position;
        public float ArriveRadius;
    }

    /// <summary>
    /// Steering 输出 → UnitMoveSystem 的积分输入。
    /// 拆成独立组件使 Steering 可降频/切片而不影响移动积分（文档 §7.3）。
    /// </summary>
    public struct SteeringVelocity : IComponentData { public float3 Value; }
}
