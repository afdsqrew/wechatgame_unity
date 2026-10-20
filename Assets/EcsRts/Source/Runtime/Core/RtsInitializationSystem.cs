using EcsRts.Combat;
using EcsRts.Movement;
using EcsRts.Orders;
using EcsRts.Selection;
using EcsRts.Spawning;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Core
{
    /// <summary>
    /// 启动兜底（文档 §4.1）：确保全部运行时单例与事件缓冲实体存在。
    /// - RtsSettings / PathfindingSettings 可能来自 SubScene 烘焙（异步加载），
    ///   故带 2 秒超时：烘焙数据正常到场则不创建，避免出现两个单例。
    /// - 其余单例没有任何 Authoring 来源，OnCreate 直接创建。
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct RtsInitializationSystem : ISystem
    {
        private float _secondsWithoutBakedSettings;

        public void OnCreate(ref SystemState state)
        {
            // 事件缓冲实体：6 个单例 DynamicBuffer 挂在同一实体上（文档 §4.3）
            state.EntityManager.CreateEntity(
                typeof(EventBuffersTag),
                typeof(DamageEvent),
                typeof(SpawnRequestEvent),
                typeof(DeathEvent),
                typeof(OrderIntentEvent),
                typeof(ProjectileSpawnEvent),
                typeof(SelectionResult));

            // 无烘焙来源的单例，直接创建
            state.EntityManager.CreateEntity(typeof(RtsCameraState));
            state.EntityManager.CreateEntity(typeof(UiSnapshot));
            state.EntityManager.CreateEntity(typeof(InputState));
            state.EntityManager.CreateEntity(typeof(SelectionRect));
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_secondsWithoutBakedSettings >= float.MaxValue)
                return; // 已完成兜底（或确认烘焙单例在场），此后不再执行

            _secondsWithoutBakedSettings += SystemAPI.Time.DeltaTime;
            if (_secondsWithoutBakedSettings < 2f)
                return;

            if (!SystemAPI.HasSingleton<RtsSettings>())
            {
                var e = state.EntityManager.CreateEntity(typeof(RtsSettings));
                state.EntityManager.SetComponentData(e, RtsSettings.Default);
                UnityEngine.Debug.LogWarning(
                    "[EcsRts] 未发现 GameSettingsAuthoring 烘焙的 RtsSettings（SubScene 未加载？），已用默认配置兜底。");
            }

            if (!SystemAPI.HasSingleton<PathfindingSettings>())
            {
                var e = state.EntityManager.CreateEntity(typeof(PathfindingSettings));
                state.EntityManager.SetComponentData(e, new PathfindingSettings
                {
                    Origin = RtsMapDefaults.MapOrigin,
                    CellSize = RtsMapDefaults.MapCellSize,
                    Dims = new int2(RtsMapDefaults.MapDim, RtsMapDefaults.MapDim),
                });
            }

            _secondsWithoutBakedSettings = float.MaxValue;
        }
    }
}
