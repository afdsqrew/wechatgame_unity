using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

    // {{ AOT assemblies
    public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
    {
        "System.dll",
        "UniFramework.Event.dll",
        "UnityEngine.CoreModule.dll",
        "YooAsset.dll",
        "mscorlib.dll",
    };
    // }}

    // {{ constraint implement type
    // }} 

    // {{ AOT generic types
    // System.Action<object>
    // System.Collections.Generic.ArraySortHelper<object>
    // System.Collections.Generic.Comparer<object>
    // System.Collections.Generic.Dictionary.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection<object,object>
    // System.Collections.Generic.Dictionary<int,object>
    // System.Collections.Generic.Dictionary<object,object>
    // System.Collections.Generic.EqualityComparer<int>
    // System.Collections.Generic.EqualityComparer<object>
    // System.Collections.Generic.ICollection<object>
    // System.Collections.Generic.IComparer<object>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<object>
    // System.Collections.Generic.IEnumerator<object>
    // System.Collections.Generic.IEqualityComparer<int>
    // System.Collections.Generic.IEqualityComparer<object>
    // System.Collections.Generic.IList<object>
    // System.Collections.Generic.KeyValuePair<int,object>
    // System.Collections.Generic.KeyValuePair<object,object>
    // System.Collections.Generic.LinkedList.Enumerator<object>
    // System.Collections.Generic.LinkedList<object>
    // System.Collections.Generic.LinkedListNode<object>
    // System.Collections.Generic.List.Enumerator<object>
    // System.Collections.Generic.List<object>
    // System.Collections.Generic.ObjectComparer<object>
    // System.Collections.Generic.ObjectEqualityComparer<int>
    // System.Collections.Generic.ObjectEqualityComparer<object>
    // System.Collections.ObjectModel.ReadOnlyCollection<object>
    // System.Comparison<object>
    // System.Predicate<object>
    // }}

    public void RefMethods()
    {
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<YooAsset.OperationAwaiter,SceneHome.<AsyncLoad>d__3>(YooAsset.OperationAwaiter&,SceneHome.<AsyncLoad>d__3&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<SceneHome.<AsyncLoad>d__3>(SceneHome.<AsyncLoad>d__3&)
        // System.Void UniFramework.Event.EventGroup.AddListener<object>(System.Action<UniFramework.Event.IEventMessage>)
        // System.Void UniFramework.Event.UniEvent.SendMessage<object>(UniFramework.Event.IEventMessage)
        // object UnityEngine.Component.GetComponent<object>()
        // object UnityEngine.GameObject.AddComponent<object>()
        // object UnityEngine.GameObject.GetComponent<object>()
        // YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
    }
}
