using UnityEngine;

public class GrassField : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int count = 500;

    Matrix4x4[] matrices;

    void Start()
    {
        matrices = new Matrix4x4[count];
        for (int i = 0; i < count; i++)
        {
            // 每个实例的位置/旋转/缩放都编码进矩阵
            Vector3 pos = new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f));
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.5f);
            matrices[i] = Matrix4x4.TRS(pos, rot, scale);
        }
    }

    void Update()
    {
        // 用 RenderParams 封装渲染状态，官方示例就是在 Update 里调用
        RenderParams rp = new RenderParams(material)
            {
                shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                receiveShadows = true
            };
        Graphics.RenderMeshInstanced(rp, mesh, 0, matrices);
    }
}