using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

public class AOTStartGame 
{
    public void OnEnter()
    {
        // Set default package.
        GameManager.Instance.SetGamePackage(YooAssets.GetPackage("DefaultPackage"));

        // Change to home scene.
        //@@@@鍔犺浇dll
        //SceneChangeToHomeEvent.SendEventMessage();
        
        Boot boot =GameManager.Instance.GetBoot();
        
        if (Application.isEditor && boot.PlayMode == EPlayMode.EditorSimulateMode)
        {
            string packageName = "HotUpdateDllsP";
            string asssetName = "HotUpdate";
            
            System.Reflection.Assembly hotUpdateAss= System.AppDomain.CurrentDomain.GetAssemblies().First( a=>a.GetName().Name==asssetName);
            Type helloType = hotUpdateAss.GetType("HotUpdateMain");
            MethodInfo helloMethod = helloType.GetMethod("Run");
            if (helloMethod != null)
                helloMethod.Invoke(null, null);
        }
        else
        {
            string packageName = "HotUpdateDllsP";
            string asssetName = "HotUpdate.dll";
            
            var assetHandle =
                GameManager.Instance.GamePackage
                    .LoadAssetAsync<TextAsset>(asssetName);

            Debug.Log("@@@AOTStartGame load assetName---noeditor mode");
            assetHandle.Completed += (AssetHandle handle) =>
            { 
                Debug.Log("@@@AOTStartGame load Complete---noeditor mode");
                
                System.Reflection.Assembly hotUpdateAss= Assembly.Load(handle.GetAssetObject<TextAsset>().bytes);
                Type helloType = hotUpdateAss.GetType("HotUpdateMain");
                MethodInfo helloMethod = helloType.GetMethod("Run");
                if (helloMethod != null)
                {
                    helloMethod.Invoke(null, null);
                }
                else
                {
                    Debug.Log("@@@@@@@@@@@@@@@@@@ if (helloMethod = null)");
                }
            };

        }
        
    }
}
