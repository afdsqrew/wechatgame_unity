using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.AI
{
    /// <summary>
    /// AI 指挥官状态（懒创建单例：本系统 OnCreate 里判断后创建，不依赖中央初始化，文档 §4.1）。
    /// </summary>
    public struct AiCommanderState : IComponentData
    {
        public float NextWaveTime;
        public int WaveIndex;
        public float3 SpawnCenter;
        public float3 AttackTarget; // 玩家主基地方向（M2 由开局标记决定）
    }

    /// <summary>
    /// 敌方指挥官（M2，文档 §7.8）：规则 AI，不做行为树。
    /// TODO(M2, §7.8)：
    ///  - 波次：NextWaveTime 到点 → 按 RtsSettings 预算写 SpawnRequestEvent（阵营 Enemy）；
    ///  - 成波：对刚生成的批次写 OrderIntentEvent（AttackMove → AttackTarget 方向）；
    ///  - 难度曲线：WaveIndex → 数量/兵种构成表。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AiCommanderSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<AiCommanderState>())
                state.EntityManager.CreateEntity(typeof(AiCommanderState));
        }

        public void OnUpdate(ref SystemState state)
        {
            // TODO(M2, §7.8)：见类注释。
        }
    }
}
