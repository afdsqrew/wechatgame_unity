using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Presentation
{
    /// <summary>
    /// 选中表现（文档 §7.9）：
    ///   M1 = 选中单位 base color 提亮（URPMaterialPropertyBaseColor 同通道，零额外几何）；
    ///   M2 = 选中环子实体（quad），随 Selected 的 ECB 增删。
    /// TODO(M1/M2, §7.9)：见类注释。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct SelectionRingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.9)：见类注释。
        }
    }
}
