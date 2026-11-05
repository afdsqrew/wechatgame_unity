using System.Collections;
using System.Collections.Generic;
using UniFramework.Event;
using UnityEngine;
using YooAsset;

public class GameManaerHotUpdate 
{
    private static GameManaerHotUpdate s_instance;
    public static GameManaerHotUpdate Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new GameManaerHotUpdate();
            return s_instance;
        }
    }
    
    private GameManaerHotUpdate()
    {
        // Register event listeners.
        GameManager.Instance.GetEventGroup().AddListener<SceneChangeToHomeEvent>(OnHandleEventMessage);
        GameManager.Instance.GetEventGroup().AddListener<SceneChangeToBattleEvent>(OnHandleEventMessage);
    }
    
    /// <summary>
    /// Handles event messages.
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        System.Type t = message.GetType();
        Debug.Log($"--- OnHandleEventMessage ={t.ToString()}");
        if (t.ToString() == typeof(SceneChangeToHomeEvent).ToString() )
        {
            GameManager.Instance.GamePackage.LoadSceneAsync("scene_home");
        }
        else if (t.ToString() == typeof(SceneChangeToBattleEvent).ToString())
        {
            Debug.Log("--- OnHandleEventMessage SceneChangeToBattleEvent 1");
            GameManager.Instance.GamePackage.LoadSceneAsync("scene_battle");
            Debug.Log("--- OnHandleEventMessage SceneChangeToBattleEvent 2");
        }
        else
        {
            Debug.Log("--- OnHandleEventMessage else 2");
        }
    }

    public void OnStartGame()
    {
      
    }
}
