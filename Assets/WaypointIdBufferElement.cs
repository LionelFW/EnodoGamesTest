using Unity.Entities;

[InternalBufferCapacity(8)]
public struct WaypointIdBufferElement : IBufferElementData
{
    public int Value;
}