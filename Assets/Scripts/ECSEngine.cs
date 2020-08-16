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
        //EntityArchetype gridArchetype = entityManager.CreateArchetype(
        //    typeof(GridComponent),
        //    typeof(Translation),
        //    typeof(RenderMesh),
        //    typeof(GridBufferElement)
        //);
        //Entity grid = entityManager.CreateEntity(gridArchetype);
        //int gridHeight = 10; int gridWidth = 10;
        //entityManager.SetComponentData(grid, new GridComponent { cellSize = 1f, isArrayPopulated = false, height = gridHeight, width = gridWidth, drawGizmos = true, gizmosDrawn = false }) ;
        //DynamicBuffer<GridBufferElement> gridBuffer = entityManager.AddBuffer<GridBufferElement>(grid);
        //entityManager.SetComponentData(grid, new GridInitComponent { width = gridWidth, height = gridHeight });

        Archetypes.spawnArchetype = entityManager.CreateArchetype(
            typeof(VehicleSpawnComponent)
        );

        Archetypes.vehicleArchetype = entityManager.CreateArchetype(
            typeof(VehicleDestinationComponent),
            typeof(VehicleSpeedComponent),
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            //typeof(RenderMesh),
            //typeof(RenderMesh),
            //typeof(RenderMesh),
            //typeof(RenderMesh),
            //typeof(RenderMesh),// Un RenderMesh pour le chassis et la carrosserie, un pour chaque roue, 
            typeof(RenderBounds),
            typeof(WaypointIdBufferElement),
            typeof(Rotation)
        );

        for(int s = 0; s < 20; s++)
        {
            
            var newSpawner = entityManager.CreateEntity(Archetypes.spawnArchetype);
            entityManager.SetComponentData(newSpawner, new VehicleSpawnComponent { spawnPos = new float3(UnityEngine.Random.Range(-500, 500), 0, UnityEngine.Random.Range(-500, 500)) });
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
