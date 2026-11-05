using System.Collections;
using UniFramework.Machine;
using UnityEngine;
using YooAsset;

internal class FSMLoadLocalPackage: IStateNode
{
    private StateMachine _machine;
    bool localSucceed = false;
    string localError = string.Empty;
    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        Debug.Log("FsmUseLocalCache");

        PatchStepChangedEvent.SendEventMessage("@@@ FsmUseLocalCache");
        
        var packageName = (string)_machine.GetBlackboardValue("PackageName");

        // Create package.
        if (!YooAssets.TryGetPackage(packageName, out var package))
            package = YooAssets.CreatePackage(packageName);
        
        GameManager.Instance.StartCoroutine(TryLoadLocalPackage(package,Callback));
    }

    private void Callback(bool succeed, string error)
    {
        localSucceed = succeed;
        localError = error;
        
        if (localSucceed)
        {
            _machine.ChangeState<FsmStartGame>();
        }
    }

    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator TryLoadLocalPackage(ResourcePackage package, System.Action<bool, string> callback)
    {
        yield return null;
        
        
        string version = PlayerPrefs.GetString("GAME_VERSION", string.Empty);
        
        Debug.Log($"TryLoadLocalPackage 1 version={version}");

        if (string.IsNullOrEmpty(version))
        {
            // 首次安装无网络时，尝试使用包体内置版本
            var builtinVersionOp = new GetBuildinPackageVersionOperation(package.PackageName);
            package.StartOperation(builtinVersionOp);
            //???
            //OperationHelper.StartOperation(package.PackageName, builtinVersionOp);
            yield return builtinVersionOp;
            if (builtinVersionOp.Status == EOperationStatus.Succeeded)
            {
                version = builtinVersionOp.PackageVersion;
            }
            else
            {
                callback.Invoke(false, builtinVersionOp.Error);
                yield break;
            }
        }
        else
        {
            Debug.Log($"TryLoadLocalPackage 2");
        }

        // 加载本地缓存的资源清单文件
        // 注意：HostPlayMode下会优先使用沙盒里已有的Hash和Manifest文件。
        var manifestOp = package.LoadPackageManifestAsync(new LoadPackageManifestOptions(version, 60,false));
        yield return manifestOp;
        if (manifestOp.Status != EOperationStatus.Succeeded)
        {
            Debug.Log($"TryLoadLocalPackage 3");
            callback.Invoke(false, manifestOp.Error);
            yield break;
        }
        else
        {
            Debug.Log($"TryLoadLocalPackage 4");
        }

        // 在正常开始游戏之前，还需要验证本地资源内容的完整性
        var downloader = package.CreateResourceDownloader(new ResourceDownloaderOptions(1, 1));
        if (downloader.TotalDownloadCount > 0)
        {
            Debug.Log($"TryLoadLocalPackage 5");
            callback.Invoke(false, "本地资源内容不完整。");
            yield break;
        }
        Debug.Log($"TryLoadLocalPackage 6");
        callback.Invoke(true, string.Empty);
    }
}
