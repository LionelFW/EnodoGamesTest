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
        var waypointCompType = GetArchetypeChunkComponentType<WaypointComponent>();
        var chunks = GetEntityQuery(ComponentType.ReadOnly<WaypointComponent>()).CreateArchetypeChunkArray(Unity.Collections.Allocator.TempJob);
        var deltaTime = Time.DeltaTime;

        var translationType = GetArchetypeChunkComponentType<Translation>(true);
        var destinationType = GetArchetypeChunkComponentType<VehicleDestinationComponent>(false);
        var speedType = GetArchetypeChunkComponentType<VehicleSpeedComponent>(true);
        var waypointType = GetArchetypeChunkComponentType<WaypointComponent>(true);
        var rotationType = GetArchetypeChunkComponentType<Rotation>(false);
        var ltwType = GetArchetypeChunkComponentType<LocalToWorld>(true);
        var waypointIdBufferType = GetArchetypeChunkBufferType<WaypointIdBufferElement>(false);

        //ComponentDataFromEntity<Translation> translationType = GetComponentDataFromEntity<Translation>(true);

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
        
        if (chunks.IsCreated) chunks.Dispose();
        

        //Entities.ForEach((ref Translation translation, ref VehicleDestinationComponent vehicleDestination, in VehicleSpeedComponent vehicleSpeed) =>
        //{

        //    int currentClosestWaypointId = -1;
        //    float currentDistanceToClosest = float.MaxValue;
        //    float3 direction = new float3();
        //    for (int i = 0; i < chunks.Length; i++)
        //    {
        //        var chunk = chunks[i];
        //        var waypoints = chunk.GetNativeArray(waypointCompType);
        //        var translations = chunk.GetNativeArray(translationType);



        //        float currentDistance = math.distance(translation.Value, translations[i].Value);

        //        if (currentDistance < currentDistanceToClosest && vehicleDestination.previousWaypointId != waypoints[i].id)
        //        {
        //            currentDistanceToClosest = currentDistance;
        //            currentClosestWaypointId = waypoints[i].id;
        //            direction = translations[i].Value;
        //        }
        //        translations.Dispose();
        //        waypoints.Dispose();
        //    }

        //    if (currentClosestWaypointId != -1) vehicleDestination.nextWaypointId = currentClosestWaypointId;
        //    translation.Value += direction * deltaTime * vehicleSpeed.speed;

        //}).ScheduleParallel();
        //chunks.Dispose();
    }

    public struct VehiclePathfindingJob : IJobChunk
    {

        public float DeltaTime;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;
        public ArchetypeChunkComponentType<VehicleDestinationComponent> destinationType;
        [ReadOnly] public ArchetypeChunkComponentType<WaypointComponent> waypointType;
        //public ArchetypeChunkComponentType<WaypointComponent> waypointType;
        [ReadOnly] public NativeArray<ArchetypeChunk> waypointChunks;
        public ArchetypeChunkBufferType<WaypointIdBufferElement> waypointIdBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            float3 direction = new float3();

            var chunkTranslation = chunk.GetNativeArray(translationType);
            var chunkDestination = chunk.GetNativeArray(destinationType);
            var waypointIdBufferAccessor = chunk.GetBufferAccessor(waypointIdBufferType);

            NativeArray<WaypointComponent> waypointChunk;


            for (int i = 0; i < chunk.Count; i++)
            {
                bool hasReachedDestination = false;
                if (chunkDestination[i].hasReachedDestination) continue;
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
                    //Je ne comprends pas pourquoi Disposer de ces NativeArrays lance des erreurs d'allocation incorrecte... 
                    //InvalidOperationException: The NativeArray can not be Disposed because it was not allocated with a valid allocator.
                    //waypointChunk.Dispose();
                }

                chunkDestination[i] = new VehicleDestinationComponent { hasReachedDestination = hasReachedDestination, direction = direction };
                
            }

        }
    }
}
