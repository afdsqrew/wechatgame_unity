using System.IO;
using UnityEngine;

namespace YooAsset
{
    /// <summary>
    /// 下载包裹哈希文件操作
    /// </summary>
    internal sealed class DownloadPackageHashOperation : AsyncOperationBase
    {
        private enum ESteps
        {
            None,
            CheckExists,
            DownloadFile,
            VerifyFile,
            Done,
        }

        private readonly SandboxFileSystem _fileSystem;
        private readonly string _packageVersion;
        private readonly int _timeout;
        private IDownloadFileRequest _downloadFileRequest;
        private string _savePath;
        private string _tempPath;
        private ESteps _steps = ESteps.None;
        private bool _requestVersionFlag ;
        
        internal DownloadPackageHashOperation(SandboxFileSystem fileSystem, string packageVersion, int timeout)
        {
            _fileSystem = fileSystem;
            _packageVersion = packageVersion;
            _timeout = timeout;
            _requestVersionFlag = true;
        }
        
        internal DownloadPackageHashOperation(SandboxFileSystem fileSystem, string packageVersion, int timeout,bool requestVersionFlag)
        {
            _fileSystem = fileSystem;
            _packageVersion = packageVersion;
            _timeout = timeout;
            _requestVersionFlag = requestVersionFlag;
        }
        
        protected override void InternalStart()
        {
            Debug.Log($"@1 hash operation _packageVersion={_packageVersion} _savePath={_savePath}");
            _savePath = _fileSystem.GetCachePackageHashFilePath(_packageVersion);
            _tempPath = _savePath + ".tmp";
            _steps = ESteps.CheckExists;
            Debug.Log($"@2 hash operation _packageVersion={_packageVersion} _savePath={_savePath}");

        }
        protected override void InternalUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.CheckExists)
            {
                //这里魔改
                if (_fileSystem.BootPlayMode == EPlayMode
                        .CustomPlayMode_HostPlayMode)
                {
                    Debug.Log($"-----这里魔改----_savePath={_savePath} _requestVersionFlag={_requestVersionFlag}");
                    if (_requestVersionFlag == false)
                    {
                        if (File.Exists(_savePath))
                        {
                            _steps = ESteps.Done;
                            SetResult();
                            Debug.Log($"-----这里魔改----1 done");
                        }
                        else
                        {
                            _steps = ESteps.DownloadFile;
                            Debug.Log($"-----这里魔改----2 done");
                        }
                    }
                    else
                    {
                        _steps = ESteps.DownloadFile;
                    }
                }
                else
                {
                    if (File.Exists(_savePath))
                    {
                        _steps = ESteps.Done;
                        SetResult();
                    }
                    else
                    {
                        _steps = ESteps.DownloadFile;
                    }
                }
            }

            if (_steps == ESteps.DownloadFile)
            {
                if (_downloadFileRequest == null)
                {
                    FileUtility.EnsureParentDirectoryExists(_tempPath);

                    // 删除历史临时文件
                    if (File.Exists(_tempPath))
                        File.Delete(_tempPath);

                    string fileName = YooAssetConfiguration.GetPackageHashFileName(_fileSystem.PackageName, _packageVersion);
                    string webURL = GetWebRequestUrl(fileName);
                    int watchdogTime = _fileSystem.DownloadWatchdogTimeout;
                    var args = new DownloadFileRequestArgs(
                        url: webURL,
                        timeout: _timeout,
                        watchdogTimeout: watchdogTime,
                        savePath: _tempPath);
                    _downloadFileRequest = _fileSystem.DownloadBackend.CreateFileRequest(args);
                    _downloadFileRequest.SendRequest();
                }

                if (_downloadFileRequest.IsDone == false)
                    return;

                if (_downloadFileRequest.Status == EDownloadRequestStatus.Succeeded)
                {
                    _steps = ESteps.VerifyFile;
                    _fileSystem.DownloadUrlPolicy.OnRequestSucceeded(_downloadFileRequest.Url);
                }
                else
                {
                    _steps = ESteps.Done;
                    SetError(_downloadFileRequest.Error);
                    _fileSystem.DownloadUrlPolicy.OnRequestFailed(_downloadFileRequest.Url, _downloadFileRequest.HttpCode, _downloadFileRequest.HttpError);
                    DeleteTempFile();
                }
            }

            if (_steps == ESteps.VerifyFile)
            {
                // 验证临时文件存在且大小有效
                FileInfo fileInfo = new FileInfo(_tempPath);
                if (fileInfo.Exists == false || fileInfo.Length == 0)
                {
                    _steps = ESteps.Done;
                    SetError("Downloaded package hash temp file is invalid.");
                    DeleteTempFile();
                    return;
                }

                // 严格校验临时文件内容
                string content = FileUtility.ReadAllText(_tempPath);
                if (TextUtility.ValidateContent(content, out string validateError) == false)
                {
                    _steps = ESteps.Done;
                    SetError($"Downloaded package hash file validation failed: {validateError}.");
                    DeleteTempFile();
                    return;
                }

                // 原子移动到最终缓存路径
                try
                {
                    if (File.Exists(_savePath))
                        File.Delete(_savePath);
                    File.Move(_tempPath, _savePath);
                    _steps = ESteps.Done;
                    SetResult();
                }
                catch (System.Exception ex)
                {
                    _steps = ESteps.Done;
                    SetError($"Failed to move hash temp file to cache path: {ex.Message}.");
                    DeleteTempFile();
                }
            }
        }
        protected override void InternalDispose()
        {
            if (_downloadFileRequest != null)
            {
                _downloadFileRequest.Dispose();
                _downloadFileRequest = null;
            }
        }

        private void DeleteTempFile()
        {
            if (File.Exists(_tempPath))
                File.Delete(_tempPath);
        }
        private string GetWebRequestUrl(string fileName)
        {
            var urls = _fileSystem.RemoteService.GetRemoteUrls(fileName);
            return _fileSystem.DownloadUrlPolicy.SelectUrl(urls);
        }
    }
}