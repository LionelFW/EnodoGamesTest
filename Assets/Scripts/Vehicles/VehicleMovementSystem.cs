using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class VehicleMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float DeltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Translation translation, ref VehicleSpeedComponent vehicleSpeed, ref VehicleDestinationComponent vehicleDestination) => {
            translation.Value += math.normalizesafe(vehicleDestination.direction) * DeltaTime * vehicleSpeed.speed;
        }).Schedule();
    }
}
