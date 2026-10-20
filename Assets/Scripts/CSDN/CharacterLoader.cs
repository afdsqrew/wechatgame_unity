using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// GetExistingSystemManaged<T>() 只接受 ComponentSystemBase 派生的托管系统；
// ISystem 是非托管 struct，实例存放在非托管内存、外部拿不到引用来调用方法，
// 因此这个供 MonoBehaviour 调用的“转发器”必须写成托管系统
partial class CharacterLoadSystem : SystemBase {
    public void ScheduleCharacterLoad(FixedString64Bytes characterId) {
        // 实际的 Addressables 加载与 GO→Entity 转换在 CharacterLoader（MonoBehaviour）中完成
        CharacterLoader.Instance.LoadCharacter(characterId.ToString());
    }

    // 没有逐帧逻辑，空实现仅为满足 SystemBase 的抽象方法要求
    protected override void OnUpdate() { }
}
 
public class CharacterLoader : MonoBehaviour {
    public static CharacterLoader Instance;
    
    Dictionary<string, Entity> _characterCache = new();

    public void Awake()
    {
        Instance = this;
    }

    public async void LoadCharacter(string id) {
        if (_characterCache.TryGetValue(id, out var entity)) {
            SpawnCharacter(entity);
            return;
        }
        
        var prefab = await AddressablesLoader.LoadPrefabAsync(id);
        var settings = GameObjectConversionSettings.FromWorld(
            World.DefaultGameObjectInjectionWorld, null);
        
        var convertedEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            prefab, settings);
        
        _characterCache[id] = convertedEntity;
        SpawnCharacter(convertedEntity);
    }
    
    void SpawnCharacter(Entity prefabEntity)
    {

        Debug.Log(
            $"@@@@@ Spawning character ");
        
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var spawnData = new DynamicRenderComponent {
            PrefabEntity = prefabEntity,
            SpawnPosition = float3.zero
        };
        // CreateEntity(typeof(...)) 只会创建零值组件，必须再 SetComponentData 写入数据，
        // 否则 DynamicSpawnSystem 会拿到 Entity.Null 去 Instantiate 而报错
        var requestEntity = entityManager.CreateEntity(typeof(DynamicRenderComponent));
        entityManager.SetComponentData(requestEntity, spawnData);
    }
}
