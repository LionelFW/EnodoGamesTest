using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Utils;
using Random = UnityEngine.Random;

public class SpawnWaypoints : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        //GameObject cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //if (waypointMaterial == null) waypointMaterial = cubePrimitive.GetComponent<Material>();
        //if (waypointMesh == null) waypointMesh = cubePrimitive.GetComponent<Mesh>();
        
        for(int s = 0; s < 50; s++)
        {
            var newWaypoint = em.CreateEntity(Archetypes.waypointArchetype);
            var x = Random.Range(-500f, 500f);
            var y = Random.Range(-500f, 500f);
            em.SetComponentData(newWaypoint, new WaypointComponent { id = UniqueID.GenerateNewId(), previousWaypoint = -1, position = new float3(x, em.GetComponentData<Scale>(newWaypoint).Value, y) });
            em.SetComponentData(newWaypoint, new Scale { Value = 1f });
            em.SetComponentData(newWaypoint, new Translation { Value = new float3(x, em.GetComponentData<Scale>(newWaypoint).Value,y) });
            em.SetSharedComponentData(newWaypoint, new RenderMesh { mesh = waypointMesh, material = waypointMaterial });
        }
        
                  
        for(int v = 0; v < 5000; v++)
        {
            var newVehicle = em.CreateEntity(Archetypes.vehicleArchetype);
            em.SetComponentData(newVehicle, new VehicleSpeedComponent { speed = UnityEngine.Random.Range(10, 25) });

            var destination = UniqueID.takenIds[UnityEngine.Random.Range(0, UniqueID.takenIds.Count)];
            em.SetComponentData(newVehicle, new VehicleDestinationComponent { destinationId = destination, nextWaypointId = -1, previousWaypointId = -1, hasReachedDestination = false });
            var entities = em.CreateEntityQuery(ComponentType.ReadOnly<VehicleSpawnComponent>()).ToEntityArray(Unity.Collections.Allocator.TempJob);
            var randomIndex = Random.Range(0, entities.Length);
            float3 spawnPos = em.GetComponentData<VehicleSpawnComponent>(entities[Random.Range(0, entities.Length)]).spawnPos;
            entities.Dispose();
            em.SetComponentData(newVehicle, new Translation { Value = spawnPos });
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
            em.SetSharedComponentData(newVehicle, new RenderMesh { mesh = vehicleMesh, material = vehicleMaterial });
            em.SetComponentData(newVehicle, new Rotation { });
        }
        
        
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
                    var newWaypoint = em.CreateEntity(Archetypes.waypointArchetype);
                    em.SetComponentData(newWaypoint, new WaypointComponent { id = UniqueID.GenerateNewId(), previousWaypoint = -1, position = new float3(hit.point.x, em.GetComponentData<Scale>(newWaypoint).Value, hit.point.z) });
                    em.SetComponentData(newWaypoint, new Scale { Value = 1f });
                    em.SetComponentData(newWaypoint, new Translation { Value = new float3(hit.point.x, em.GetComponentData<Scale>(newWaypoint).Value, hit.point.z) });
                    em.SetSharedComponentData(newWaypoint, new RenderMesh { mesh = waypointMesh, material = waypointMaterial });
                    //em.SetComponentData(newWaypoint, new RenderBounds { Value = waypointMesh.bounds.ToAABB() });
                }
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            var newVehicle = em.CreateEntity(Archetypes.vehicleArchetype);
            em.SetComponentData(newVehicle, new VehicleSpeedComponent { speed = UnityEngine.Random.Range(1, 10) });

            var destination = UniqueID.takenIds[UnityEngine.Random.Range(0, UniqueID.takenIds.Count)];
            em.SetComponentData(newVehicle, new VehicleDestinationComponent { destinationId = destination, nextWaypointId = -1, previousWaypointId = -1, hasReachedDestination = false });
            var entities = em.CreateEntityQuery(ComponentType.ReadOnly<VehicleSpawnComponent>()).ToEntityArray(Unity.Collections.Allocator.TempJob);
            var randomIndex = Random.Range(0, entities.Length);
            float3 spawnPos = em.GetComponentData<VehicleSpawnComponent>(entities[Random.Range(0, entities.Length)]).spawnPos;
            entities.Dispose();
            em.SetComponentData(newVehicle, new Translation { Value = spawnPos });
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
            em.SetSharedComponentData(newVehicle, new RenderMesh { mesh = vehicleMesh, material = vehicleMaterial });
        }
    }
}
