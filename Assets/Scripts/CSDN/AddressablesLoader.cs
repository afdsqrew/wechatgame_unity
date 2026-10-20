using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AddressablesLoader : MonoBehaviour
{
    public static async Task<GameObject> LoadPrefabAsync(
        string key)
    {
        var handle =
            Addressables.LoadAssetAsync<GameObject>(key);
        await handle.Task;
        return handle.Status ==
               AsyncOperationStatus.Succeeded
            ? handle.Result
            : null;
    }
}