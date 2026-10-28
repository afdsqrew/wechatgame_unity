using System;
using System.Linq;
using System.Reflection;
using UniFramework.Machine;
using YooAsset;
using Application = UnityEngine.Application;

internal class FsmStartGame : IStateNode
{
    void IStateNode.OnCreate(StateMachine machine)
    {
    }
    void IStateNode.OnEnter()
    {
        PatchStepChangedEvent.SendEventMessage("Starting game.");

        // Set default package.
        GameManager.Instance.SetGamePackage(YooAssets.GetPackage("DefaultPackage"));

        // Change to home scene.
        //@@@@加载dll
        //SceneChangeToHomeEvent.SendEventMessage();
        
         string packageName = "HotUpdateDllsP";
         string asssetName = "HotUpdate";

         if (Application.isEditor)
         {
             System.Reflection.Assembly hotUpdateAss= System.AppDomain.CurrentDomain.GetAssemblies().First( a=>a.GetName().Name==asssetName);
             Type helloType = hotUpdateAss.GetType("HotUpdateMain");
             MethodInfo helloMethod = helloType.GetMethod("Run");
             if (helloMethod != null)
                 helloMethod.Invoke(null, null);
         }
         else
         {
             /*
             System.Reflection.Assembly hotUpdateAss;
             Type helloType = hotUpdateAss.GetType("HelloWorld");
             MethodInfo helloMethod = helloType.GetMethod("Run");
             if (helloMethod != null)
                 helloMethod.Invoke(null, null);
                 */
         }

    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }
}