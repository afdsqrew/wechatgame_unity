using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EcsRts.Selection
{
    /// <summary>
    /// 旧版 Input Manager 轮询（项目未装 Input System 包，文档 §1）。
    /// 托管 SystemBase：UnityEngine.Input 非 Burst 兼容，故集中受控在此，
    /// 其余系统只读 InputState 单例，不再触输入 API。
    /// 更新顺序：SimulationSystemGroup OrderFirst（与 SpatialGridSystem 并列最早）。
    /// TODO(M1)：Input.touches 触摸适配（单指=左键/点击、双指=平移+捏合缩放，WebGL 移动端）。
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class RtsInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!SystemAPI.HasSingleton<InputState>())
                return;

            var input = SystemAPI.GetSingleton<InputState>();
            var mouse = Input.mousePosition;
            var newPos = new float2(mouse.x, mouse.y);

            input.MouseDeltaPx = newPos - input.MousePositionPx;
            input.MousePositionPx = newPos;
            input.LeftDown = Input.GetMouseButtonDown(0) ? (byte)1 : (byte)0;
            input.LeftHeld = Input.GetMouseButton(0) ? (byte)1 : (byte)0;
            input.LeftUp = Input.GetMouseButtonUp(0) ? (byte)1 : (byte)0;
            input.RightDown = Input.GetMouseButtonDown(1) ? (byte)1 : (byte)0;
            input.MiddleDown = Input.GetMouseButtonDown(2) ? (byte)1 : (byte)0;
            input.ScrollDelta = Input.GetAxisRaw("Mouse ScrollWheel");
            input.PanAxes = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input.ShiftDown = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? (byte)1 : (byte)0;
            input.CtrlDown = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) ? (byte)1 : (byte)0;

            SystemAPI.SetSingleton(input);
        }
    }
}
