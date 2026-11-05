using UniFramework.Event;
using UnityEngine;

/// <summary>
/// Change to home scene event.
/// </summary>
public sealed class SceneChangeToHomeEvent : IEventMessage
{
    public static void SendEventMessage()
    {
        UniEvent.SendMessage(new SceneChangeToHomeEvent());
    }
}

/// <summary>
/// Change to battle scene event.
/// </summary>
public sealed class SceneChangeToBattleEvent : IEventMessage
{
    public static void SendEventMessage()
    {
        Debug.Log("@@@change2scenebatle 1");
        UniEvent.SendMessage<SceneChangeToBattleEvent>(new SceneChangeToBattleEvent());
        Debug.Log("@@@change2scenebatle 2");
    }
}