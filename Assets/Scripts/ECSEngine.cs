using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Utils;
using Unity.Mathematics;

public class ECSEngine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        UniqueID.takenIds = new List<int>();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Archetypes.spawnArchetype = entityManager.CreateArchetype(
            typeof(VehicleSpawnComponent)
        );

        Archetypes.vehicleArchetype = entityManager.CreateArchetype(
            typeof(VehicleDestinationComponent),
            typeof(VehicleSpeedComponent),
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(WaypointIdBufferElement),
            typeof(Rotation)
        );

        for(int s = 0; s < 10; s++)
        {
            var newSpawner = entityManager.CreateEntity(Archetypes.spawnArchetype);
            entityManager.SetComponentData(newSpawner, new VehicleSpawnComponent { spawnPos = new float3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20)) });
        }

        Archetypes.waypointArchetype = entityManager.CreateArchetype(
            typeof(WaypointComponent),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(RenderBounds)
        );

    }
}
