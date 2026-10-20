using EcsRts.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Combat
{
    /// <summary>生命值。Current ≤ 0 由 DeathSystem 处理（文档 §7.4）。</summary>
    public struct Health : IComponentData
    {
        public float Current;
        public float Max;
    }

    /// <summary>攻击参数。M1 远程与近战同为即时伤害（弹道 M2 实体化，文档 §7.4）。</summary>
    public struct AttackStats : IComponentData
    {
        public float Damage;
        public float Range;
        public float CooldownSeconds;
        public byte IsMelee;
        public float ProjectileSpeed; // M2 生效
    }

    /// <summary>攻击冷却剩余（AttackExecutionSystem 每帧全量 tick）。</summary>
    public struct AttackCooldownTimer : IComponentData { public float Remaining; }

    /// <summary>当前攻击目标实体。TargetAcquisitionSystem 写；失效置 Entity.Null。</summary>
    public struct AttackTarget : IComponentData { public Entity Value; }

    /// <summary>仇恨半径（索敌用）。</summary>
    public struct AggroRange : IComponentData { public float Value; }

    /// <summary>投射物运行时数据（M2 实体化；由 ProjectileSpawnEvent 生成，文档 §7.4）。</summary>
    public struct Projectile : IComponentData
    {
        public Entity Source;
        public Entity Target;
        public float3 Velocity;
        public float Damage;
        public float Ttl;
    }

    /// <summary>伤害事件（单例缓冲，文档 §4.3）：近战/弹道命中写 → DamageApplicationSystem 消费。</summary>
    public struct DamageEvent : IBufferElementData
    {
        public Entity Target;
        public Entity Source;
        public float Amount;
        public float3 HitPosition;
    }

    /// <summary>远程开火事件（单例缓冲）：AttackExecutionSystem 写 → ProjectileMoveSystem 实体化（M2）。</summary>
    public struct ProjectileSpawnEvent : IBufferElementData
    {
        public Entity Source;
        public Entity Target;
        public float Speed;
        public float Damage;
    }

    /// <summary>死亡事件（单例缓冲）：DeathSystem 写 → 呈现/统计/AI 消费。</summary>
    public struct DeathEvent : IBufferElementData
    {
        public Entity DiedEntity;
        public Faction Faction;
        public UnitType UnitType;
        public float3 Position;
    }
}
