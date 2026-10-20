using EcsRts.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EcsRts.Presentation
{
    /// <summary>
    /// RTS 相机桥（GameObject 侧，文档 §2/§7.9）：WASD/方向键平移 + 滚轮缩放。
    /// LateUpdate 把相机状态写入 RtsCameraState 单例，供 Burst 系统做屏幕投影/地面射线；
    /// 消费者容忍 1 帧滞后（文档 §2）。
    /// TODO(M3, §7.9)：边缘滚动、Q/E 旋转、触摸双指（WebGL 移动端）、平滑阻尼、地图边界限制。
    /// </summary>
    public sealed class RtsCameraRig : MonoBehaviour
    {
        public float panSpeed = 40f;
        public float zoomSpeed = 80f;

        private Camera _camera;
        private EntityQuery _stateQuery;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
                // SetSingleton<T> 内部走 GetSingletonRW，查询必须具备读写访问（ReadOnly 会抛 InvalidOperationException）
                _stateQuery = world.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<RtsCameraState>());
        }

        private void LateUpdate()
        {
            var dt = Time.deltaTime;
            var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
            var move = Input.GetAxisRaw("Horizontal") * right + Input.GetAxisRaw("Vertical") * forward;
            transform.position += move * (panSpeed * dt);
            transform.position += forward * (Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed);

            Publish();
        }

        private void Publish()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || _camera == null || _stateQuery == null)
                return;
            if (!_stateQuery.HasSingleton<RtsCameraState>())
                return;

            _stateQuery.SetSingleton(new RtsCameraState
            {
                Position = transform.position,
                Forward = transform.forward,
                Up = transform.up,
                Right = transform.right,
                FovYRadians = _camera.fieldOfView * Mathf.Deg2Rad,
                OrthoSize = _camera.orthographic ? _camera.orthographicSize : 0f,
                Aspect = _camera.aspect,
                IsOrtho = _camera.orthographic ? (byte)1 : (byte)0,
            });
        }
    }
}
