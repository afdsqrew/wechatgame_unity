using UnityEngine;
using UniFramework.Event;
using YooAsset;

public class Boot : MonoBehaviour
{
    /// <summary>
    /// Resource system play mode.
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
    public PatchWindow PatchWindowPrefab;
    public bool UseLocalUrlCDN = false;
    public DownloadUrlWrapper downloadUrlWrapper = new DownloadUrlWrapper();
    void Awake()
    {
        Debug.Log($"Resource system play mode: {PlayMode}.");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        downloadUrlWrapper.SetUseLocalCDNFlag(UseLocalUrlCDN);
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        // Game manager.
        GameManager.Instance.SetBehaviour(this);
        GameManager.Instance.SetBoot(this);
        // Initialize event system.
        UniEvent.Initalize();
        
        // Initialize asset system.
        YooAssets.Initialize();

        // Load patch window.
        var go = PatchWindowPrefab ;
        GameObject.Instantiate(go);

        // Start patch workflow.
        PatchManager.Create("DefaultPackage", PlayMode);
        PatchManager.Start();
    }
    private void Update()
    {
        PatchManager.Update();
    }
}
