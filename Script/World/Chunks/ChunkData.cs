using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public Voxel[] voxels;
    public Vector3Int worldPosition;
    public World worldReference;
    public Dictionary<Vector3, WalkablePosition> walkablePositions;

    public ChunkData(World worldReference, Vector3Int worldPosition)
    {
        this.worldReference = worldReference;
        this.worldPosition = worldPosition;
    }
}