using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

public class GridSystem : ComponentSystem
{
    EntityManager manager;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        //var ecb = 
        //Entities.ForEach((ref GridComponent gridComponent, DynamicBuffer<GridBufferElement> gridBuffer) =>
        //{
        //    if (!gridComponent.isArrayPopulated) PopulateGridArray(gridComponent, gridBuffer);
        //});
        ////In that project, there always only be a single grid
        //Entities.ForEach((ref Translation translation, ref GridComponent gridComponent) =>
        //{
        //    if (!gridComponent.gizmosDrawn)
        //    {
        //        DrawGizmos(gridComponent, translation);
        //        gridComponent.gizmosDrawn = true;
        //    };
        //});
        
    }

    // Calculating world position from grid indexes. Because we're in 3D, the array's Y (or GridComponent.gridArray's second dimension) corresponds to the World's Z axis. 
    private float3 GetWorldPosition(GridComponent grid, int arrayX, int arrayY)
    {
        return new float3(arrayX, 0f, arrayY) * grid.cellSize;
    }

    private void PopulateGridArray(GridComponent gridComponent, DynamicBuffer<GridBufferElement> gridBuffer)
    {
        for(int x = 0; x < gridComponent.width; x++)
        {
            for (int y = 0; y < gridComponent.height; y++)
            {
                gridBuffer[x * gridComponent.width + y] = 0;
            }
        }
        gridComponent.isArrayPopulated = true;
    }
    
    //private int Get2DArrayValue(GridComponent grid, int x, int y)
    //{
    //}

    public float3 GetNearestPointOnGrid(Translation gridTranslation, GridComponent gridComponent, float3 position)
    {
        position -= gridTranslation.Value;

        int xCount = Mathf.RoundToInt(position.x / gridComponent.cellSize);
        int yCount = Mathf.RoundToInt(position.y / gridComponent.cellSize);
        int zCount = Mathf.RoundToInt(position.z / gridComponent.cellSize);

        float3 result = new float3(
            (float)xCount * gridComponent.cellSize,
            (float)yCount * gridComponent.cellSize,
            (float)zCount * gridComponent.cellSize);

        result += gridTranslation.Value;

        return result;
    }

    private void DrawGizmos(GridComponent gridComponent, Translation translation)
    {
        //if (!gridComponent.drawGizmos) return;
        //for(int x = 0; x < gridComponent.gridArray.GetLength(0); x++)
        //{
        //    for (int y = 0; y < gridComponent.gridArray.GetLength(1); y++)
        //    {
        //        GameObject.CreatePrimitive(PrimitiveType.Quad);
        //    }
        //}
    }
}
