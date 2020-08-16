using Unity.Entities;

//InternalBufferCapacity is arbitrary here. Vehicles will not visit more than 8 waypoints.
[InternalBufferCapacity(8)]
public struct WaypointIdBufferElement : IBufferElementData
{
    public int Value;
}