
using UnityEngine;

namespace YooAsset
{
    /// <summary>
    /// 加载资源清单操作
    /// </summary>
    public sealed class LoadPackageManifestOperation : AsyncOperationBase
    {
        private enum ESteps
        {
            None,
            CheckParams,
            CheckActiveManifest,
            LoadPackageManifest,
            Done,
            LoadPackageManifest_RequestVersionFlagFalse
        }

        private readonly FileSystemHost _host;
        private readonly LoadPackageManifestOptions _options;
        private FSLoadPackageManifestOperation _loadPackageManifestOp;
        private ESteps _steps = ESteps.None;

        internal LoadPackageManifestOperation(FileSystemHost host, LoadPackageManifestOptions options)
        {
            _host = host;
            _options = options;
        }
        /// <inheritdoc />
        protected override void InternalStart()
        {
            if (!_options.RequestPackageVersionFlag)
            {
                Debug.Log($"@进入自定义流程 !!!");
                _steps = ESteps.LoadPackageManifest_RequestVersionFlagFalse;
            }
            else
            {
                _steps = ESteps.CheckParams;
            }
        }
        /// <inheritdoc />
        protected override void InternalUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.CheckParams)
            {
                if (!_options.RequestPackageVersionFlag)
                {
                    Debug.Log($"@进入自定义流程 _options.RequestPackageVersionFlag={_options.RequestPackageVersionFlag}");
                    _steps = ESteps.CheckActiveManifest;
                }
                else
                {
                    if (string.IsNullOrEmpty(_options.PackageVersion))
                    {
                        _steps = ESteps.Done;
                        SetError("Package version is null or empty.");
                    }
                    else
                    {
                        _steps = ESteps.CheckActiveManifest;
                    }
                }
            }

            if (_steps == ESteps.CheckActiveManifest)
            {
                // 检测当前激活的清单对象	
                if (_host.ActiveManifest != null && _host.ActiveManifest.PackageVersion == _options.PackageVersion)
                {
                    Debug.Log("@2 LOAD PACKAGE MAN 1");
                    _steps = ESteps.Done;
                    SetResult();
                }
                else
                {
                    Debug.Log("@2 LOAD PACKAGE MAN 2");
                    _steps = ESteps.LoadPackageManifest;
                }
            }

            if (_steps == ESteps.LoadPackageManifest)
            {
                if (_loadPackageManifestOp == null)
                {
                    var mainFileSystem = _host.GetMainFileSystem();
                    if (mainFileSystem == null)
                    {
                        _steps = ESteps.Done;
                        SetError("Main file system is null.");
                        return;
                    }

                    _loadPackageManifestOp = mainFileSystem.LoadPackageManifestAsync(_options.ConvertTo());
                    _loadPackageManifestOp.StartOperation();
                    AddChildOperation(_loadPackageManifestOp);
                }

                _loadPackageManifestOp.UpdateOperation();
                Progress = _loadPackageManifestOp.Progress;
                if (_loadPackageManifestOp.IsDone == false)
                    return;

                if (_loadPackageManifestOp.Status == EOperationStatus.Succeeded)
                {
                    _steps = ESteps.Done;
                    _host.SetActiveManifest(_loadPackageManifestOp.Manifest);
                    SetResult();
                }
                else
                {
                    _steps = ESteps.Done;
                    SetError(_loadPackageManifestOp.Error);
                }
            }
            
            if (_steps == ESteps.LoadPackageManifest_RequestVersionFlagFalse)
            {
                if (_loadPackageManifestOp == null)
                {
                    Debug.Log("@32 LOAD PACKAGE MAN 2");
                    var mainFileSystem = _host.GetMainFileSystem();
                    if (mainFileSystem == null)
                    {
                        _steps = ESteps.Done;
                        SetError("Main file system is null.");
                        return;
                    }

                    _loadPackageManifestOp = mainFileSystem.LoadPackageManifestAsync(_options.ConvertToMethod2());
                    _loadPackageManifestOp.StartOperation();
                    AddChildOperation(_loadPackageManifestOp);
                    Debug.Log("@33 LOAD PACKAGE MAN 2");
                }

                //卡这里了
                _loadPackageManifestOp.UpdateOperation();
                Progress = _loadPackageManifestOp.Progress;
                if (_loadPackageManifestOp.IsDone == false)
                    return;

                Debug.Log("@35 LOAD PACKAGE MAN 2");
                if (_loadPackageManifestOp.Status == EOperationStatus.Succeeded)
                {
                    Debug.Log("@36 LOAD PACKAGE MAN 2");
                    _steps = ESteps.Done;
                    _host.SetActiveManifest(_loadPackageManifestOp.Manifest);
                    SetResult();
                    Debug.Log("@37 LOAD PACKAGE MAN 2");
                }
                else
                {
                    Debug.Log("@38 LOAD PACKAGE MAN 2");
                    _steps = ESteps.Done;
                    SetError(_loadPackageManifestOp.Error);
                    Debug.Log("@39 LOAD PACKAGE MAN 2");
                }
            }
        }
        /// <inheritdoc />
        protected override string InternalGetDescription()
        {
            return $"PackageVersion: {_options.PackageVersion}";
        }
    }
}