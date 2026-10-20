using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class BatchRendererGroupSetup : MonoBehaviour
{
    private BatchRendererGroup brg;
    // Start is called before the first frame update
    public Mesh mesh;
    public Material material;

    private BatchMeshID meshID;
    private BatchMaterialID materialID;
    
    void Start()
    {
        brg = new BatchRendererGroup(this.OnPerformCulling,
                IntPtr.Zero)
            ;

        meshID = brg.RegisterMesh(mesh);
        materialID = brg.RegisterMaterial(material);
    }

    private JobHandle OnPerformCulling(BatchRendererGroup renderergroup, BatchCullingContext cullingcontext, BatchCullingOutput cullingoutput, IntPtr usercontext)
    {
        return new JobHandle();
    }

    private void OnDestroy()
    {
        brg.Dispose();
    }


}
