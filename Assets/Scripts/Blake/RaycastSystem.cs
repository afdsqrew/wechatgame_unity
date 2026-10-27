using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;

namespace Blake
{
    /// <summary>
    /// system里面获取singleton
    /// </summary>
    public partial class RaycastSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                this.ScreenRaycast();
            }
        }

        private void ScreenRaycast()
        {
            
            UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput hit = new RaycastInput()
            {
                Start = ray.origin,
                End = ray.GetPoint(100000f),
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            
            
        }
    }
}
