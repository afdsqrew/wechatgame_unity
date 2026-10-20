using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public float Health;
}

public class UnitBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        // TransformUsageFlags.Dynamic：位置旋转运行时会变（敌人移动）
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
    }
}