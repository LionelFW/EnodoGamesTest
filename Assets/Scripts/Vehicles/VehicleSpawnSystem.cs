using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

public class VehicleSpawnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var waypointCompType = GetArchetypeChunkComponentType<WaypointComponent>();
        var translationType = GetArchetypeChunkComponentType<Translation>();
        var chunks = GetEntityQuery(ComponentType.ReadOnly<WaypointComponent>(), ComponentType.ReadOnly<Translation>()).CreateArchetypeChunkArray(Unity.Collections.Allocator.TempJob);
        Entities.ForEach((ref VehicleSpawnComponent spawn) => {
            //if(math.ceil(Time.ElapsedTime) == 0)
            //{
            //    if (chunks.Length != 0)
            //    {
            //        var newVehicle = EntityManager.CreateEntity(Archetypes.vehicleArchetype);
            //        EntityManager.SetComponentData(newVehicle, new VehicleSpeedComponent { speed = UnityEngine.Random.Range(1, 10) });
            //        var waypoints = chunks[0].GetNativeArray(waypointCompType);
            //        EntityManager.SetComponentData(newVehicle, new VehicleDestinationComponent { destinationId = waypoints[UnityEngine.Random.Range(0, waypoints.Length)].id, nextWaypointId = -1 });
            //        EntityManager.SetComponentData(newVehicle, new Translation { Value = spawn.spawnPos });
            //        waypoints.Dispose();
            //    }
            //}
        });
        chunks.Dispose();
    }
}
