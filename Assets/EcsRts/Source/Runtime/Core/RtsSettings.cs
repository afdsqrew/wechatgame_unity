using Unity.Entities;
using UnityEngine;

namespace EcsRts.Core
{
    /// <summary>
    /// 全局配置单例（文档 §4.1/§9）。
    /// 优先由 SubScene 内的 GameSettingsAuthoring 烘焙；缺失超时后 RtsInitializationSystem 用 Default 兜底。
    /// </summary>
    public struct RtsSettings : IComponentData
    {
        public PerformanceTier Tier;
        public int UnitCap;                   // 同屏单位上限
        public float UnitRadius;              // 通用单位半径（避让/拾取/阵型槽间距）
        public byte TargetingSlices;           // 索敌分帧片数
        public float TargetingIntervalSeconds; // 每片持续时长；全轮 = Slices × Interval
        public byte FlowFieldBudgetPerFrame;   // 每帧流场重建预算
        public int SpawnBudgetPerFrame;        // 每帧实例化预算（防结构变更尖峰）
        public int ProjectileCap;              // 场上投射物上限（M2 实体化后生效）

        public static RtsSettings Default => new RtsSettings
        {
            Tier = PerformanceTier.Medium,
            UnitCap = 8000,
            UnitRadius = 0.5f,
            TargetingSlices = 8,
            TargetingIntervalSeconds = 0.15f,
            FlowFieldBudgetPerFrame = 1,
            SpawnBudgetPerFrame = 256,
            ProjectileCap = 600,
        };
    }

    /// <summary>SubScene 内挂一个（仅一个）。Inspector 调参并烘焙为单例。</summary>
    [DisallowMultipleComponent]
    public class GameSettingsAuthoring : MonoBehaviour
    {
        [Header("分级（参数表见 Docs/RTS-Architecture.md §9）")]
        public PerformanceTier tier = PerformanceTier.Medium;
        [Header("规模")]
        public int unitCap = 8000;
        public float unitRadius = 0.5f;
        [Header("性能预算")]
        public byte targetingSlices = 8;
        public float targetingIntervalSeconds = 0.15f;
        public byte flowFieldBudgetPerFrame = 1;
        public int spawnBudgetPerFrame = 256;
        public int projectileCap = 600;

        public RtsSettings ToSettings() => new RtsSettings
        {
            Tier = tier,
            UnitCap = unitCap,
            UnitRadius = unitRadius,
            TargetingSlices = targetingSlices,
            TargetingIntervalSeconds = targetingIntervalSeconds,
            FlowFieldBudgetPerFrame = flowFieldBudgetPerFrame,
            SpawnBudgetPerFrame = spawnBudgetPerFrame,
            ProjectileCap = projectileCap,
        };
    }

    public class GameSettingsBaker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            // 单例：烘焙到本 GameObject 的实体上；全场景只允许一个 GameSettingsAuthoring
            AddComponent(GetEntity(TransformUsageFlags.None), authoring.ToSettings());
        }
    }
}
