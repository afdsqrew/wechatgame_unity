using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Selection
{
    /// <summary>可被选中。Radius 用于点击拾取的屏幕空间/世界半径容差。</summary>
    public struct Selectable : IComponentData
    {
        public float Radius;
        public byte Priority;
    }

    /// <summary>
    /// 选中态。按 enableable tag 声明；M1 由 SelectionApplySystem 用 ECB 增删，
    /// M2 切换为 SetComponentEnabled 消除结构变更（文档 §4.4）。
    /// </summary>
    public struct Selected : IComponentData, IEnableableComponent { }

    /// <summary>悬停态（渲染高亮用，M1 后期）。</summary>
    public struct Hovered : IComponentData, IEnableableComponent { }

    /// <summary>旧版输入轮询快照（RtsInputSystem 每帧写，文档 §7.5）。</summary>
    public struct InputState : IComponentData
    {
        public float2 MousePositionPx; // 屏幕像素，左下原点
        public float2 MouseDeltaPx;
        public byte LeftDown;           // 本帧按下
        public byte LeftHeld;           // 按住中
        public byte LeftUp;             // 本帧松开
        public byte RightDown;
        public byte MiddleDown;
        public float ScrollDelta;
        public float2 PanAxes;          // WASD / 方向键
        public byte ShiftDown;
        public byte CtrlDown;
    }

    /// <summary>当前框选矩形（屏幕像素，左下原点）。BoxSelectionSystem 维护。</summary>
    public struct SelectionRect : IComponentData
    {
        public byte IsActive;
        public float2 StartPx;
        public float2 EndPx;
    }

    /// <summary>拾取结果事件缓冲：Box/ClickSelection 写 → SelectionApplySystem 消费（文档 §4.3）。</summary>
    public struct SelectionResult : IBufferElementData
    {
        public Entity Entity;
        public byte ShouldSelect;
    }
}
