using EcsRts.Core;
using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Presentation
{
    /// <summary>
    /// 血条渲染（文档 §7.9）：
    ///   M1 = 单位子实体四边形 + 每实例 URPMaterialPropertyBaseColor（绿→红按血量比例）；
    ///   只对“受损单位”开启渲染；Low 档再叠加相机距离裁剪（读 RtsCameraState）。
    ///   M3 参考 历史演示 HealthBarRenderSystem 的 BRG 直写思路做合批优化（自定义 RenderMeshArray）。
    /// TODO(M1, §7.9)：
    ///  - 单位烘焙时带一个隐藏的血条子实体（Authoring 里拖引用）；
    ///  - IJobEntity：按 Health 比例写颜色；满血禁用（ECB / Disabled enableable）；
    ///  - 性能分级读取 RtsSettings.Tier 决定距离裁剪。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct HealthBarRenderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<RtsSettings>())
                return;

            // TODO(M1, §7.9)：见类注释。
        }
    }
}
