using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;

[WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Presentation)]
public partial class TextureInstanceCollectSystem : SystemBase
{
    private List<float4x4> _matrixList = new();

    protected override void OnUpdate()
    {
        _matrixList.Clear();
    }
}