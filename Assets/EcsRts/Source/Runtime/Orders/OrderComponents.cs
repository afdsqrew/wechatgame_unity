using EcsRts.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace EcsRts.Orders
{
    /// <summary>单位当前指令（每单位一条，文档 §7.5）。OrderIssueSystem 写，OrderStatusSystem 维护生命周期。</summary>
    public struct UnitOrder : IComponentData
    {
        public OrderType Type;
        public Entity TargetEntity; // AttackTarget 有效；失效置 Entity.Null
        public float3 Destination;  // Move/AttackMove 目标点
        public float ArriveRadius;  // 到达判定半径
    }

    /// <summary>
    /// 玩家指令意图（单例事件缓冲，文档 §4.3）：
    /// 输入翻译层每次操作写一条（描述“这次点击”），OrderIssueSystem 展开为全体选中单位的 UnitOrder。
    /// </summary>
    public struct OrderIntentEvent : IBufferElementData
    {
        public OrderType Type;
        public Entity TargetEntity; // AttackTarget 用
        public float3 TargetPosition;
    }
}
