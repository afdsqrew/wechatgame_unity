
/*
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YooAsset;

public class LoadDll : MonoBehaviour
{
    private TextAsset dllText;
    private string packageName = "HotUpdateDllsP";
    private string asssetName = "HotUpdateDlll";
    
    // Start is called before the first frame update
    void Start()
    {
        
        Assembly hotUpdateAsset = System.Reflection.Assembly.Load(dllText.bytes);
        Type helloType = hotUpdateAsset.GetType("HelloWorld");
        MethodInfo helloMethod = helloType.GetMethod("Run");
        helloMethod.Invoke(null, null);
        
    }

    IEnumerable StartedHot()
    {
        YooAssets.Initialize();
        bool isok= YooAssets.TryGetPackage(packageName,out var packageObj);

        var package = isok
            ? packageObj
            : YooAssets.CreatePackage(packageName);
        
        //无联网
        var createParam = new OfflinePlayModeOptions();
  
        
        Assembly hotUpdateAsset = System.Reflection.Assembly.Load(dllText.bytes);
        Type helloType = hotUpdateAsset.GetType("HelloWorld");
        MethodInfo helloMethod = helloType.GetMethod("Run");
        helloMethod.Invoke(null, null);
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/