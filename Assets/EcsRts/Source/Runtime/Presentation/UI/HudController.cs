using EcsRts.Core;
using Unity.Entities;
using UnityEngine;

namespace EcsRts.Presentation.UI
{
    /// <summary>
    /// HUD（GameObject 侧，文档 §2/§7.9）：只读 UiSnapshot 单例做展示。
    /// TODO(M1, §7.9)：在场景建 Canvas + TMPro 文本并挂引用，显示 单位数/选中数/FPS。
    /// TODO(M2)：选择面板、生产按钮、资源栏——届时扩充 UiSnapshot 字段（文档 §7.9）。
    /// 注意：uGUI/TMP 程序集引用已在 EcsRts.Runtime.asmdef 预留（UnityEngine.UI / Unity.TextMeshPro）。
    /// </summary>
    public sealed class HudController : MonoBehaviour
    {
        private EntityQuery _snapshotQuery;
        private float _fpsSmoothed;

        private void OnEnable()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null)
                _snapshotQuery = world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<UiSnapshot>());
        }

        private void Update()
        {
            if (Time.deltaTime > 0f)
                _fpsSmoothed = Mathf.Lerp(_fpsSmoothed, 1f / Time.deltaTime, 0.05f);

            if (_snapshotQuery == null || !_snapshotQuery.HasSingleton<UiSnapshot>())
                return;

            var s = _snapshotQuery.GetSingleton<UiSnapshot>();
            // TODO(M1, §7.9)：把 s.PlayerUnitCount / s.EnemyUnitCount / s.SelectedCount / _fpsSmoothed 绑到 UI 文本。
        }
    }
}
