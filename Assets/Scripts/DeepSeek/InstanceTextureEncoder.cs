using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class InstanceTextureEncoder
{
    // 复用的像素缓冲：SetPixels 要求长度精确等于 width*height，只在尺寸变化时重建，
    // 避免每帧 new Color[]（每实例 4 个 Color）产生的 GC 压力
    private static Color[] _scratch;

    // 将 NativeArray<Matrix4x4> 编码到 Texture2D
    // 每个矩阵占 4 个像素，每个像素为一个 float4
    public static Texture2D EncodeMatrices(NativeArray<float4x4> matrices, Texture2D existing = null)
    {
        int instanceCount = matrices.Length;
        int pixelCount = instanceCount * 4;

        // 计算纹理尺寸：宽固定为 256（足够缓存友好），高向上取整
        int width = 256;
        int height = math.max(1, (pixelCount + width - 1) / width);
        // 确保总像素数是 4 的倍数
        int totalPixels = width * height;
        if (totalPixels % 4 != 0)
            height = (pixelCount + 3) / 4 / width + 1;
        totalPixels = width * height;

        Texture2D tex;
        if (existing == null || existing.width != width || existing.height != height)
        {
            if (existing != null) Object.Destroy(existing);
            tex = new Texture2D(width, height, TextureFormat.RGBAHalf, false, true)
            {
                filterMode = FilterMode.Point,   // 必须 Point，避免插值破坏矩阵数据
                wrapMode = TextureWrapMode.Clamp,
                name = "InstanceMatrixTexture"
            };
        }
        else
        {
            tex = existing;
        }

        if (_scratch == null || _scratch.Length != totalPixels)
            _scratch = new Color[totalPixels];

        var pixels = _scratch;

        for (int i = 0; i < instanceCount; i++)
        {
            float4x4 m = matrices[i];
            // Unity Matrix4x4 的列主序：c0 c1 c2 c3
            // 每列是一个 float4，正好对应一个像素
            int b = i * 4;
            pixels[b + 0] = new Color(m.c0.x, m.c0.y, m.c0.z, m.c0.w);
            pixels[b + 1] = new Color(m.c1.x, m.c1.y, m.c1.z, m.c1.w);
            pixels[b + 2] = new Color(m.c2.x, m.c2.y, m.c2.z, m.c2.w);
            pixels[b + 3] = new Color(m.c3.x, m.c3.y, m.c3.z, m.c3.w);
        }
        // 缓冲复用后尾部可能残留上一帧的旧矩阵，清掉未被实例占用的像素
        for (int i = pixelCount; i < totalPixels; i++)
            pixels[i] = default;

        tex.SetPixels(pixels);
        tex.Apply(false, false); // 不生成 mipmap
        return tex;
    }
}
