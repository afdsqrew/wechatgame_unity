using UnityEngine;
using UnityEngine.Rendering;

public class TextureInstancedRenderer : System.IDisposable
{
    private Material _material;
    private Mesh     _mesh;
    private Texture2D _instanceTex;
    private int      _instanceCount;

    // 可选：指定绘制层，需与相机 cullingMask 匹配，默认 Default(0)
    private int _layer = 0;

    private static readonly int InstanceTexId       = Shader.PropertyToID("_InstanceTex");
    private static readonly int InstanceTexWidthId  = Shader.PropertyToID("_InstanceTexWidth");
    private static readonly int InstanceOffsetId    = Shader.PropertyToID("_InstanceOffset");

    // 单次 draw call 建议上限，超过则拆批次
    private const int MaxPerDraw = 1024;

    public TextureInstancedRenderer(Material material, Mesh mesh)
    {
        _material = material;
        _mesh     = mesh;

        if (!_material.enableInstancing)
        {
            Debug.LogWarning($"[TextureInstancedRenderer] Material '{_material.name}' 未开启 Enable GPU Instancing，" +
                             $"DrawMeshInstancedProcedural 可能不会生效。");
        }
    }

    public void UpdateInstances(Texture2D tex, int count)
    {
        if (tex == null) return;
        _instanceTex   = tex;
        _instanceCount = count;

        _material.SetTexture(InstanceTexId, _instanceTex);
        _material.SetFloat(InstanceTexWidthId, _instanceTex.width);
    }

    public void Draw(Camera camera)
    {
        if (_instanceTex == null || _instanceCount == 0 || _material == null || _mesh == null)
            return;

        if (!SystemInfo.supportsInstancing)
        {
            DrawFallback(camera);
            return;
        }

        // bounds 尽量贴合实际实例分布范围；过大会导致整体包围盒无法被视锥剔除
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        int remaining = _instanceCount;
        int offset    = 0;

        while (remaining > 0)
        {
            int batch = Mathf.Min(remaining, MaxPerDraw);

            // 多批次时告知 Shader 本次 Draw 的起始实例索引
            // SV_InstanceID 每次 Draw 从 0 重新计数，必须加偏移
            _material.SetFloat(InstanceOffsetId, offset);

            Graphics.DrawMeshInstancedProcedural(
                _mesh,                          // mesh
                0,                              // submeshIndex
                _material,                      // material
                bounds,                         // bounds
                batch,                          // count
                null,                           // properties (MaterialPropertyBlock)
                ShadowCastingMode.On,           // castShadows
                false,                          // receiveShadows（WebGL 下建议关，避免额外开销）
                _layer,                         // layer
                camera,                         // camera
                LightProbeUsage.Off,            // lightProbeUsage（小游戏建议 Off）
                null                            // lightProbeProxyVolume
            );

            remaining -= batch;
            offset    += batch;
        }
    }

    private void DrawFallback(Camera camera)
    {
        Debug.LogWarning("[TextureInstancedRenderer] 当前平台不支持 GPU Instancing，走 fallback 路径。");
    }

    public void Dispose()
    {
        if (_instanceTex != null)
        {
            Object.Destroy(_instanceTex);
            _instanceTex = null;
        }
    }
}