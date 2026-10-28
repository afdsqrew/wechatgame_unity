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
        if (message is SceneChangeToHomeEvent)
        {
            GameManager.Instance.GamePackage.LoadSceneAsync("scene_home");
        }
        else if (message is SceneChangeToBattleEvent)
        {
            GameManager.Instance.GamePackage.LoadSceneAsync("scene_battle");
        }
    }

    public void OnStartGame()
    {
      
    }
}
