using UnityEngine;
using YooAsset;

public class SceneHome : MonoBehaviour
{
    public GameObject CanvasDesktop;

    private AssetHandle _windowHandle;

    void Start()
    {
        Debug.Log("hotupdate --SceneHome --start");
        AsyncLoad();
    }

    private async void AsyncLoad()
    {
        Debug.Log("hotupdate --SceneHome --AsyncLoad 1");
        // Load home window.
        _windowHandle = GameManager.Instance.GamePackage.LoadAssetAsync<GameObject>("UIHome");
        await _windowHandle;
        Debug.Log("hotupdate --SceneHome --AsyncLoad 2");
        _windowHandle.InstantiateSync(new InstantiateOptions(true, CanvasDesktop.transform, false));

        // Release unused assets after changing scenes.
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadUnusedAssetsAsync();
        await operation;
        Debug.Log("hotupdate --SceneHome --AsyncLoad 3");
    }

    private void OnDestroy()
    {
        // Release asset handle.
        if (_windowHandle != null)
        {
            _windowHandle.Release();
            _windowHandle = null;
        }
    }
}