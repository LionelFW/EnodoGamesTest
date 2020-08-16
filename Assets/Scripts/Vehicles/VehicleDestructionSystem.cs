using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class VehicleDestructionSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        EntityCommandBuffer cmd = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        Entities.ForEach((Entity entity, ref VehicleDestinationComponent vehicleDestination) =>
        {
            if (vehicleDestination.hasReachedDestination)
                cmd.DestroyEntity(entity);
        }).Run();
    }
}
