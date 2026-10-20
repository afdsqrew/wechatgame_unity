using EcsRts.Combat;
using EcsRts.Core;
using EcsRts.Movement;
using EcsRts.Orders;
using EcsRts.Selection;
using Unity.Entities;
using UnityEngine;

namespace EcsRts.Authoring
{
    /// <summary>
    /// 单位数值配置（文档 §3 场景规划）：挂在 SubScene 内的“单位 prefab GameObject”本体上
    /// （prefab 本体也放在 SubScene 里，由 UnitPrefabLibraryAuthoring 引用并烘焙成实体 prefab）。
    /// 运行时由 UnitSpawnerSystem 实例化；带 RenderMesh 的 prefab 由 Entities Graphics 渲染。
    /// M1 美术可直接用图元（Cube/Capsule）拼单位；正式模型 M2+ 再换（文档 §10）。
    /// </summary>
    [DisallowMultipleComponent]
    public class UnitAuthoring : MonoBehaviour
    {
        public UnitType unitType = UnitType.Soldier;
        public Faction faction = Faction.Player;

        [Header("生存与移动")]
        public float maxHealth = 100f;
        public float moveSpeed = 4f;

        [Header("战斗")]
        public float attackDamage = 10f;
        public float attackRange = 1.8f;
        public float attackCooldownSeconds = 1f;
        public float aggroRange = 12f;
        public bool isMelee = true;
        public float projectileSpeed = 20f; // M2 远程弹道用
    }

    public class UnitBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitTag());
            AddComponent(entity, new FactionId { Value = authoring.faction });
            AddComponent(entity, new Health { Current = authoring.maxHealth, Max = authoring.maxHealth });
            AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
            AddComponent(entity, new MoveTarget { Position = authoring.transform.position, ArriveRadius = 0.5f });
            AddComponent(entity, new SteeringVelocity());
            AddComponent(entity, new Selectable { Radius = 0.6f, Priority = 1 });
            AddComponent(entity, new AggroRange { Value = authoring.aggroRange });
            AddComponent(entity, new AttackStats
            {
                Damage = authoring.attackDamage,
                Range = authoring.attackRange,
                CooldownSeconds = authoring.attackCooldownSeconds,
                IsMelee = authoring.isMelee ? (byte)1 : (byte)0,
                ProjectileSpeed = authoring.projectileSpeed,
            });
            AddComponent(entity, new AttackCooldownTimer { Remaining = 0f });
            AddComponent(entity, new AttackTarget { Value = Entity.Null });
            AddComponent(entity, new UnitOrder { Type = OrderType.None, ArriveRadius = 0.5f });

            // Selected 故意不在烘焙时添加：IEnableableComponent 默认处于“启用”态，烘焙即加 = 全图默认选中。
            // 由 SelectionApplySystem 首次选中时经 ECB 添加；M2 切 SetComponentEnabled（文档 §4.4）。
            // UpdateSliceIndex 在此仅占位，UnitSpawnerSystem 实例化后按计数器均匀重排（文档 §4.2）。
            AddComponent(entity, new UpdateSliceIndex { Value = 0 });
        }
    }
}
