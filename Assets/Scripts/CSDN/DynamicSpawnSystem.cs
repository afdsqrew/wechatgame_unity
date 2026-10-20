using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

struct DynamicRenderComponent : IComponentData {
    public Entity PrefabEntity;
    public float3 SpawnPosition;
}
 
[BurstCompile]
partial struct DynamicSpawnSystem : ISystem {
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (renderComp, entity) in 
                 SystemAPI.Query<DynamicRenderComponent>()
                     .WithEntityAccess()) {
            
            var instance = ecb.Instantiate(renderComp.PrefabEntity);
            ecb.SetComponent(instance, 
                LocalTransform.FromPosition(renderComp.SpawnPosition));
            ecb.DestroyEntity(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
