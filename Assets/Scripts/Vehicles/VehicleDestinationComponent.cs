using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct VehicleDestinationComponent : IComponentData
{
    public int destinationId;
    public bool hasReachedDestination;
    public float3 direction;
}
