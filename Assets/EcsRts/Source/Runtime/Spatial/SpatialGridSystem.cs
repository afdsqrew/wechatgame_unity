using EcsRts.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Spatial
{
    /// <summary>
    /// 均匀网格的全局只读视图（SpatialGridSystem 每帧重建，文档 §7.1）。
    /// 消费方式：调度 Job 时把 Cells 句柄“拷贝”进 Job 字段（禁止在 Job 内访问静态成员）。
    /// 域重载安全：OnCreate 重分配、OnDestroy 带 IsCreated 保护（文档 §13）。
    /// </summary>
    public static class SpatialGridState
    {
        public static NativeParallelMultiHashMap<int, Entity> Cells;
        public static float3 Origin;
        public static float CellSize;
        public static int2 Dims;

        public static int HashCell(int2 cell) => cell.y * Dims.x + cell.x;

        // 地面网格按 XZ 平面索引（y 恒 0，不参与分格）
        public static int2 WorldToCell(float3 world)
        {
            var rel = (world - Origin) / CellSize;
            return new int2((int)math.floor(rel.x), (int)math.floor(rel.z));
        }
    }

    /// <summary>
    /// 每帧把所有 UnitTag 单位插入均匀网格（消费者：索敌/避让/拾取/投射物/血条裁剪，文档 §7.1）。
    /// 更新顺序：SimulationSystemGroup 内 OrderFirst，先于一切消费者。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct SpatialGridSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            SpatialGridState.Origin = RtsMapDefaults.MapOrigin;
            SpatialGridState.CellSize = RtsMapDefaults.SpatialCellSize;
            SpatialGridState.Dims = new int2(RtsMapDefaults.SpatialDim, RtsMapDefaults.SpatialDim);
            SpatialGridState.Cells = new NativeParallelMultiHashMap<int, Entity>(16384, Allocator.Persistent);
        }

        public void OnDestroy(ref SystemState state)
        {
            if (SpatialGridState.Cells.IsCreated)
                SpatialGridState.Cells.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.1)：
            //  1. SpatialGridState.Cells.Clear();
            //  2. IJobEntity：Query RefRO<LocalTransform> WithAll<UnitTag>，
            //     hash = SpatialGridState.HashCell(SpatialGridState.WorldToCell(pos))，
            //     经 NativeParallelMultiHashMap<int, Entity>.ParallelWriter 并行写入；
            //     —— 把 writer 传 Job 字段，静态成员只在主线程读。
            //  3. 若场上为空可跳过（SystemAPI.QueryBuilder 缓存 EntityQuery，OnCreate 里建）。
        }
    }
}
