using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

public class TextureInstancedRenderer : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    public int textureSize = 256; // 一行存 256/4=64个实例
    private RenderTexture _instanceTex;
    private Color[] _pixelData;
    private List<float4x4> _matrices = new();
    private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 100);

    void Awake()
    {
        // RGBAHalf 16bit，WebGL2支持
        //@ _instanceTex = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RGBAHalf);
        _instanceTex.filterMode = FilterMode.Point;
        _instanceTex.wrapMode = TextureWrapMode.Clamp;
        _pixelData = new Color[textureSize * textureSize];
        mat.SetTexture("_InstanceTex", _instanceTex);
        mat.SetFloat("_TexSize", textureSize);
    }

    /// <summary>
    /// DOTS每帧调用，提交所有实例世界矩阵
    /// </summary>
    public void SetInstances(List<float4x4> worldMatrices)
    {
        _matrices = worldMatrices;
        int count = _matrices.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            var m = _matrices[i];
            int pxBase = i * 4; // 每个实例占4个texel（4行矩阵）
            if (pxBase + 3 >= _pixelData.Length) break;

            _pixelData[pxBase + 0] = new Color(m.c0.x, m.c0.y, m.c0.z, m.c0.w);
            _pixelData[pxBase + 1] = new Color(m.c1.x, m.c1.y, m.c1.z, m.c1.w);
            _pixelData[pxBase + 2] = new Color(m.c2.x, m.c2.y, m.c2.z, m.c2.w);
            _pixelData[pxBase + 3] = new Color(m.c3.x, m.c3.y, m.c3.z, m.c3.w);
        }
        // 上传CPU数据到GPU纹理
        RenderTexture.active = _instanceTex;
        //@@ _instanceTex.SetPixels(_pixelData);
        //@@ _instanceTex.Apply();
        RenderTexture.active = null;
    }

    void OnRenderObject()
    {
        if (_matrices.Count == 0 || mesh == null || mat == null) return;
        mat.SetPass(0);
        //@ Graphics.DrawMeshInstanced(mesh, 0, mat, _bounds, _matrices.Count);
    }

    void OnDestroy()
    {
        Destroy(_instanceTex);
    }
}
