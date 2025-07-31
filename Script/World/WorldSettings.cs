using UnityEngine;

public static class WorldSettings
{
    public const int numberOfThreads = 8;

    public const float voxelSize = .25f;
    public const int feetInChunk = 4;

    public const int renderDistanceInChunks = 4;
    public static int GetRenderDistance()
    {
        return renderDistanceInChunks;
    }

    public static int TotalVoxelsInAxis()
    {
        return (int)(feetInChunk / voxelSize);
    }

    public static int TotalVoxelInChunk()
    {
        int totalVoxelsInAxis = TotalVoxelsInAxis();
        return totalVoxelsInAxis * totalVoxelsInAxis * totalVoxelsInAxis;
    }

    public static int TotalVoxelInChunkWithNeighbours()
    {
        int totalVoxelsInAxis = TotalVoxelsInAxis() + 2;
        return totalVoxelsInAxis * totalVoxelsInAxis * totalVoxelsInAxis;
    }

    public static int ThreadGroups()
    {
        int totalVoxelsInAxis = TotalVoxelsInAxis();
        return totalVoxelsInAxis / numberOfThreads;
    }

    public static int IndexFromCoord(int x, int y, int z)
    {
        int totalVoxelsInAxis = TotalVoxelsInAxis();
        return x + totalVoxelsInAxis * (y + totalVoxelsInAxis * z);
    }

    public static int IndexFromCoordWithNeighbours(int x, int y, int z)
    {
        int totalVoxelsInAxis = TotalVoxelsInAxis() + 2;
        return (x + 1) + totalVoxelsInAxis * ((y + 1) + totalVoxelsInAxis * (z + 1));
    }

    public static Vector3 CoordFromIndex(int index)
    {
        int totalVoxelInAxis = TotalVoxelsInAxis();

        int z = index / (totalVoxelInAxis * totalVoxelInAxis);
        int remaining = index % (totalVoxelInAxis * totalVoxelInAxis);

        int y = remaining / totalVoxelInAxis;
        int x = remaining % totalVoxelInAxis;

        return new Vector3(x, y, z);
    }

    public static void SetWorldSettings(ComputeShader shader)
    {
        shader.SetFloat("voxelSize", voxelSize);
        shader.SetInt("feetInChunk", feetInChunk);
    }
}