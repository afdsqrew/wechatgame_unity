using Unity.Burst;
using Unity.Entities;

namespace EcsRts.Selection
{
    /// <summary>
    /// 框选（文档 §7.5）：
    ///   LeftDown 记起点 → LeftHeld 更新 SelectionRect（供 UI 画框）→ LeftUp 结算。
    ///   小于 4px 的拖动视为点击，转交 ClickSelectionSystem。
    /// TODO(M1, §7.5)：
    ///  - 结算时用 RtsCameraState + float4x4.PerspectiveFov/Ortho 手工构建 VP 矩阵（Burst 兼容）；
    ///  - IJobEntity 对全体 Selectable 并行做世界→屏幕投影，落点在矩形内者写入 SelectionResult；
    ///  - Shift 追加保留原选中；M1 不做 Ctrl 反选。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RtsInputSystem))]
    public partial struct BoxSelectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.5)：见类注释。
        }
    }
}
