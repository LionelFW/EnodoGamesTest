using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct GridComponent : IComponentData
{
    public float cellSize;
    public int height;
    public int width;
    public bool isArrayPopulated;
    
    // Debug properties
    public bool drawGizmos;
    public bool gizmosDrawn;
}

public struct GridInitComponent : IComponentData
{
    public int height;
    public int width;
}

[InternalBufferCapacity(100)]
public struct GridBufferElement : IBufferElementData
{
    public static implicit operator int(GridBufferElement e) { return e.Value; }
    public static implicit operator GridBufferElement(int e) { return new GridBufferElement { Value = e }; }

    // Actual value each buffer element will store.
    public int Value;
}
