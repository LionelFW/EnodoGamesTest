using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Utils;
using Random = UnityEngine.Random;

public class SpawnEntities : MonoBehaviour
{
    EntityManager em;
    [SerializeField]
    private Mesh waypointMesh;
    [SerializeField]
    private Material waypointMaterial;
    [SerializeField]
    private Mesh vehicleMesh;
    [SerializeField]
    private Material vehicleMaterial;
    [SerializeField]
    [Range(0f, 5f)]
    private float spawnRate = 0.2f;

    float chrono = 0f;

    // Start is called before the first frame update
    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
              
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("EditZone"))
                {
                    SpawnWaypoint(hit.point);
                }
            }

        }

        chrono += Time.deltaTime;
        // We only start spawning vehicles when 2 or more waypoints are created in the scene
        var query = em.CreateEntityQuery(ComponentType.ReadOnly<WaypointComponent>()).ToEntityArray(Unity.Collections.Allocator.TempJob);
        if(query.Length > 1 && chrono > spawnRate)
        {
            SpawnVehicle();
            chrono = 0f;
        }
        query.Dispose();
    }

    public void SpawnVehicle()
    {
        var newVehicle = em.CreateEntity(Archetypes.vehicleArchetype);

        em.SetComponentData(newVehicle, 
            new VehicleSpeedComponent { speed = UnityEngine.Random.Range(10, 25) }
        );

        var destination = UniqueID.takenIds[UnityEngine.Random.Range(0, UniqueID.takenIds.Count)];
        em.SetComponentData(newVehicle, 
            new VehicleDestinationComponent { destinationId = destination, hasReachedDestination = false }
        );

        var entities = em.CreateEntityQuery(ComponentType.ReadOnly<VehicleSpawnComponent>()).ToEntityArray(Unity.Collections.Allocator.TempJob);
        var randomIndex = Random.Range(0, entities.Length);
        float3 spawnPos = em.GetComponentData<VehicleSpawnComponent>(entities[Random.Range(0, entities.Length)]).spawnPos;
        entities.Dispose();
        
        em.SetComponentData(newVehicle,
            new Translation { Value = spawnPos }
        );
        
        var dynamicBuffer = em.AddBuffer<WaypointIdBufferElement>(newVehicle);
        int previousWaypoint = -1;
        for (int w = 0; w < Random.Range(1, dynamicBuffer.Capacity); w++)
        {
            int waypoint;
            do
            {
                waypoint = UniqueID.takenIds[Random.Range(0, UniqueID.takenIds.Count)];
            } while (previousWaypoint == waypoint);
            dynamicBuffer.Add(new WaypointIdBufferElement { Value = waypoint });
            previousWaypoint = waypoint;
        }

        em.SetSharedComponentData(newVehicle, 
            new RenderMesh { mesh = vehicleMesh, material = vehicleMaterial }
        );

        em.SetComponentData(newVehicle, 
            new Rotation { }
        );
    }

    private void SpawnWaypoint(Vector3 pos)
    {
        var newWaypoint = em.CreateEntity(Archetypes.waypointArchetype);

        em.SetComponentData(newWaypoint, 
            new WaypointComponent { id = UniqueID.GenerateNewId(), position = new float3(pos.x, em.GetComponentData<Scale>(newWaypoint).Value, pos.z) }
        );

        em.SetComponentData(newWaypoint, 
            new Scale { Value = 1f }
        );

        em.SetComponentData(newWaypoint, 
            new Translation { Value = new float3(pos.x, em.GetComponentData<Scale>(newWaypoint).Value, pos.z) } 
        );

        em.SetSharedComponentData(newWaypoint, 
            new RenderMesh { mesh = waypointMesh, material = waypointMaterial }
        );
    }
}
