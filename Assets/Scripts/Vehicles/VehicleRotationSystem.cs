using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class VehicleRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        Entities.ForEach((ref VehicleDestinationComponent vehicleDestination, ref Rotation rotation) => {
            rotation.Value = quaternion.LookRotationSafe(vehicleDestination.direction, math.up());
        }).Schedule();
    }
}
