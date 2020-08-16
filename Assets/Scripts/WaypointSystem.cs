using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class WaypointSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        Entities.ForEach((ref WaypointComponent waypointComponent, ref Translation waypointTranslation) =>
        {

        });
    }
}

public struct MeshDrawingJob : IJob
{
    public void Execute()
    {
        Vector3[] vertices = new Vector3[0];
        Vector2[] uv = new Vector2[0];
        int[] triangles = new int[0];

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
    }
}
