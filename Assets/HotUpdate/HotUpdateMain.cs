using System;
using System.Reflection;
using UnityEngine;

public class HotUpdateMain  
{
    public static void Run()
    {
        Debug.Log("Hello World!");
        GameManaerHotUpdate.Instance.OnStartGame();
        
        SceneChangeToHomeEvent.SendEventMessage();
    }
}
