using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct WaypointComponent : IComponentData
{
    public int id;
    public int previousWaypoint;
    public float3 position;
}
