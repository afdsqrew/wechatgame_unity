using EcsRts.Combat;
using EcsRts.Orders;
using EcsRts.Selection;
using EcsRts.Spawning;
using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Core
{
    /// <summary>
    /// 标记“事件缓冲实体”：RtsInitializationSystem 创建，承载全部单例事件缓冲（文档 §4.3）。勿手工再建。
    /// </summary>
    public struct EventBuffersTag : IComponentData { }

    /// <summary>
    /// 帧末清空全部事件缓冲（文档 §4.3，OrderLast）。
    /// 约束：事件的生产与消费都发生在 SimulationSystemGroup 内、本系统之前。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct EventBufferClearSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<EventBuffersTag>())
                return;

            SystemAPI.GetSingletonBuffer<DamageEvent>(false).Clear();
            SystemAPI.GetSingletonBuffer<SpawnRequestEvent>(false).Clear();
            SystemAPI.GetSingletonBuffer<DeathEvent>(false).Clear();
            SystemAPI.GetSingletonBuffer<OrderIntentEvent>(false).Clear();
            SystemAPI.GetSingletonBuffer<ProjectileSpawnEvent>(false).Clear();
            SystemAPI.GetSingletonBuffer<SelectionResult>(false).Clear();
        }
    }
}
