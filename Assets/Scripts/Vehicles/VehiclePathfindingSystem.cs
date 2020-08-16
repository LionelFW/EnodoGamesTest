using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class VehiclePathfindingSystem : SystemBase
{
    public EntityQuery m_Query;
    protected override void OnCreate()
    {
        base.OnCreate();
        var queryDescription1 = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(VehicleDestinationComponent) }
        };

        m_Query = GetEntityQuery(queryDescription1);
    }

    protected override void OnUpdate()
    {

        // Because Waypoints and Vehicles are different entities, we first obtain the chunks that contain the Waypoint Components. Those chunks will be passed to the VehiclePathfindingJob.
        // This job will in turn use components of the Vehicle Entity, and iterate on both of those chunks to read and write data.
        var chunks = GetEntityQuery(ComponentType.ReadOnly<WaypointComponent>()).CreateArchetypeChunkArray(Unity.Collections.Allocator.TempJob);
        var deltaTime = Time.DeltaTime;

        //Marking ArchetypeChunkComponentTypes as ReadOnly is important for performance
        var translationType = GetArchetypeChunkComponentType<Translation>(true);
        var destinationType = GetArchetypeChunkComponentType<VehicleDestinationComponent>(false);
        var waypointType = GetArchetypeChunkComponentType<WaypointComponent>(true);
        var waypointIdBufferType = GetArchetypeChunkBufferType<WaypointIdBufferElement>(false);

        var job = new VehiclePathfindingJob()
        {
            DeltaTime = deltaTime,
            translationType = translationType,
            destinationType = destinationType,
            waypointType = waypointType,
            waypointChunks = chunks,
            waypointIdBufferType = waypointIdBufferType,
        };
        
        this.Dependency = job.Schedule(m_Query, this.Dependency);
        this.Dependency.Complete();
        
        //Disposing of NativeArrays ensure that no memory leak happens.
        if (chunks.IsCreated) chunks.Dispose();
    }

    public struct VehiclePathfindingJob : IJobChunk
    {

        public float DeltaTime;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
        public ArchetypeChunkComponentType<VehicleDestinationComponent> destinationType;
        [ReadOnly] public ArchetypeChunkComponentType<WaypointComponent> waypointType;
        [ReadOnly] public NativeArray<ArchetypeChunk> waypointChunks;
        public ArchetypeChunkBufferType<WaypointIdBufferElement> waypointIdBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            float3 direction = new float3();

            var chunkTranslation = chunk.GetNativeArray(translationType);
            var chunkDestination = chunk.GetNativeArray(destinationType);
            var waypointIdBufferAccessor = chunk.GetBufferAccessor(waypointIdBufferType);

            NativeArray<WaypointComponent> waypointChunk;

            // First, we iterate on the chunks with the Translation component, the VehicleDestinationComponent, and the WaypointId DynamicBuffers.
            for (int i = 0; i < chunk.Count; i++)
            {
                bool hasReachedDestination = false;
                if (chunkDestination[i].hasReachedDestination) continue;

                // Then, we iterate on the chunks with the Waypoint component. We compare the next destination id and the current waypoint id.
                for (int j = 0; j < waypointChunks.Length; j++)
                {
                    waypointChunk = waypointChunks[j].GetNativeArray<WaypointComponent>(waypointType);
                    for (int g = 0; g < waypointChunk.Length; g++)
                    {
                        if (waypointIdBufferAccessor[i].Length > 0)
                        {
                            if (waypointChunk[g].id.Equals(waypointIdBufferAccessor[i][0].Value))
                            {
                                if (math.distance(waypointChunk[g].position, chunkTranslation[i].Value) < 0.5f)
                                {
                                    waypointIdBufferAccessor[i].RemoveAt(0);
                                    continue;
                                }
                                direction = waypointChunk[g].position - chunkTranslation[i].Value;
                            }
                        } else
                        {
                            hasReachedDestination = true;
                        }
                        
                        
                    }

                    //Why can't we dispose of NativeArrays obtained via GetNativeArray() without throwing exceptions ?
                    //InvalidOperationException: The NativeArray can not be Disposed because it was not allocated with a valid allocator.
                    //waypointChunk.Dispose();
                }

                chunkDestination[i] = new VehicleDestinationComponent { hasReachedDestination = hasReachedDestination, direction = direction };
                
            }

        }
    }
}
