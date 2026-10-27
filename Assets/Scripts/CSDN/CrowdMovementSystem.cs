using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// 人群自由移动数据：CrowdSpawnSystem 在 MoveSpeed>0 时挂到每个实例上
public struct CrowdMovement : IComponentData
{
    public float3 Velocity;
    public float MoveSpeed;
    public float DirectionChangeInterval;   // 平均换向间隔
    public float DirectionTimer;            // 距离下次换向的倒计时
    public uint RngState;                   // 换向随机数状态（0 非法，创建时已 |1u）
    public float3 SpawnCenter;              // 活动范围圆心
    public float BoundsRadius;              // 超出该半径会把方向拉回圆心，防止人群无限散开
}

// 放在 TransformSystemGroup 最前：先写 LocalTransform，组内的 ParentSystem/LocalToWorldSystem
// 随后级联算出世界矩阵，TextureInstanceRenderSystem（Presentation 阶段）当帧就能拿到新位置
[BurstCompile]
[UpdateInGroup(typeof(TransformSystemGroup), OrderFirst = true)]
partial struct CrowdMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (movement, transform) in
                 SystemAPI.Query<RefRW<CrowdMovement>, RefRW<LocalTransform>>())
        {
            var m = movement.ValueRW;
            m.DirectionTimer -= dt;


            movement.ValueRW = m;
            transform.ValueRW.Position += m.Velocity * dt;
        }
    }
}
