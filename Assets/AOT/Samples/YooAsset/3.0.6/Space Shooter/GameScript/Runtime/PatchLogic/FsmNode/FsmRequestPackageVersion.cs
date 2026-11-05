using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

internal class FsmRequestPackageVersion : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchStepChangedEvent.SendEventMessage("Requesting package version.");
        GameManager.Instance.StartCoroutine(UpdatePackageVersion());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator UpdatePackageVersion()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var playMode = (EPlayMode)_machine.GetBlackboardValue("PlayMode");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeeded)
        {
            _machine.SetBlackboardValue("RequestPackageVersionFlag",false);
            Debug.LogWarning($"所以远端流程中任意一步失败，都不要直接保存版本号，而是交给本地兜底流程处理。");
            if (playMode == EPlayMode.CustomPlayMode_HostPlayMode)
            {
                //不能这么做要先 FsmUpdatePackageManifest=》 var operation = package.LoadPackageManifestAsync(options);
                //_machine.ChangeState<FsmClearCacheBundle>();
                _machine.ChangeState<FSMLoadLocalPackage>();
            }
            else
            {
                PatchPackageVersionRequestFailedEvent
                    .SendEventMessage();
            }
        }
        else
        {
            _machine.SetBlackboardValue("RequestPackageVersionFlag",true);
            Debug.Log($"Package version requested: '{operation.PackageVersion}'.");
            _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}