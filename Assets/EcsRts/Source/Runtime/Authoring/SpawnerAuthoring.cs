using EcsRts.Core;
using EcsRts.Spawning;
using Unity.Entities;
using UnityEngine;

namespace EcsRts.Authoring
{
    /// <summary>
    /// 开局批量刷兵（文档 §7.6）：挂 SubScene 内任意 GameObject（每阵营一个）。
    /// 烘焙为 InitialSpawnRequest，由 UnitSpawnerSystem 每帧按预算分批转成 SpawnRequestEvent。
    /// </summary>
    [DisallowMultipleComponent]
    public class SpawnerAuthoring : MonoBehaviour
    {
        public UnitType unitType = UnitType.Soldier;
        public Faction faction = Faction.Enemy;
        public int count = 1000;
        public float spawnRadius = 20f;
    }

    public class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new InitialSpawnRequest
            {
                UnitType = authoring.unitType,
                Faction = authoring.faction,
                Count = authoring.count,
                Center = authoring.transform.position,
                Radius = authoring.spawnRadius,
            });
        }
    }

    /// <summary>
    /// UnitType → prefab 登记表（文档 §5）：数组下标即 UnitType 枚举值（0=Soldier, 1=Archer, 2=Worker）。
    /// 单位 prefab 本体放同一 SubScene，挂 UnitAuthoring。
    /// </summary>
    [DisallowMultipleComponent]
    public class UnitPrefabLibraryAuthoring : MonoBehaviour
    {
        public GameObject[] unitPrefabs;
    }

    public class UnitPrefabLibraryBaker : Baker<UnitPrefabLibraryAuthoring>
    {
        public override void Bake(UnitPrefabLibraryAuthoring authoring)
        {
            var buf = AddBuffer<UnitPrefabLibrary>(GetEntity(TransformUsageFlags.None));
            for (int i = 0; i < authoring.unitPrefabs.Length; i++)
            {
                var prefab = authoring.unitPrefabs[i];
                if (prefab == null)
                    continue;
                buf.Add(new UnitPrefabLibrary
                {
                    UnitType = (UnitType)i,
                    Prefab = GetEntity(prefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}
