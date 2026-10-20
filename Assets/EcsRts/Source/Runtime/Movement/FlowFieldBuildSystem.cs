using EcsRts.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Movement
{
    /// <summary>
    /// 流场构建（文档 §7.2）：
    ///   目的地聚类 → 每帧预算 FlowFieldBudgetPerFrame 次重建 → BFS 灌积分场 → 导出流向场。
    ///   单位侧 O(1) 采样流向，是 1w+ 单位共享寻路的核心。
    /// TODO(M1, §7.2)：
    ///  - 收集在途 MoveTarget（M1：取最新一个目的地建单字段；M2：≤8 字段 LRU 池）；
    ///  - BFS brushfire 写 _integration（ushort 距离），跳过阻挡位图置位格；
    ///  - 导出每格 float2 流向到 _flow（越界/不可达格置零）；
    ///  - 全部在单 Burst Job 内完成（65k 格亚毫秒级，WASM 上也安全）；
    ///  - 采样接口：供 SteeringSystem 读取 _flow[cell]（静态只读视图 + Job 句柄拷贝）。
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ObstacleGridSystem))]
    public partial struct FlowFieldBuildSystem : ISystem
    {
        private NativeArray<ushort> _integration; // 到目标格的积分距离
        private NativeArray<float2> _flow;        // 每格流向（单位化）
        private int2 _fieldDims;

        public void OnCreate(ref SystemState state)
        {
            _fieldDims = new int2(RtsMapDefaults.MapDim, RtsMapDefaults.MapDim);
            _integration = new NativeArray<ushort>(_fieldDims.x * _fieldDims.y, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _flow = new NativeArray<float2>(_fieldDims.x * _fieldDims.y, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_integration.IsCreated) _integration.Dispose();
            if (_flow.IsCreated) _flow.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            // TODO(M1, §7.2)：见类注释。
        }
    }
}
