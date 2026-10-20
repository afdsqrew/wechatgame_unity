using EcsRts.Combat;
using EcsRts.Core;
using EcsRts.Selection;
using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Presentation
{
    /// <summary>
    /// 统计快照（文档 §7.9）：IJobEntity 双阵营计数（UnitTag + FactionId）与投射物计数
    /// → SystemAPI.SetSingleton&lt;UiSnapshot&gt;；FPS 由 HudController 侧自算（Time API 非 Burst 兼容）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct StatsOverlaySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.9)：见类注释。
        }
    }
}
