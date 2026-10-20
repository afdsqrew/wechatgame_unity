using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EntityProxyAuthoring : MonoBehaviour {
    public GameObject TargetPrefab;
    
    void Start() {
        StartCoroutine(InitializeEntity());
    }
 
    IEnumerator InitializeEntity() {
        var request = Addressables.LoadAssetAsync<GameObject>(TargetPrefab.name);
        yield return request;
        
        if (request.Result != null) {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            var entity = entityManager.CreateEntity();
            
            entityManager.AddComponentData(entity, new RenderProxyComponent {
                PrefabEntity = RuntimeGoToEntityConverter.ConvertHierarchy(
                    request.Result, 
                    GameObjectConversionSettings.FromWorld(world, null))
            });
        }
    }
}