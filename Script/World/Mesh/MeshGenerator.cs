using System.Runtime.InteropServices;
using UnityEngine;

public static class MeshGenerator
{
    struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 uvA;
        public Vector3 uvB;
        public Vector3 uvC;

        public static int SizeOf => sizeof(float) * 3 * 6;
    }

    public static ComputeShader meshGeneratorShader;

    public static ComputeBuffer chunkVoxelsBuffer;

    public static ComputeBuffer trianglesBuffer;
    public static ComputeBuffer trianglesCountBuffer;

    private static void InitializeShader()
    {
        meshGeneratorShader = Resources.Load<ComputeShader>("Shaders/World/meshGeneratorShader");
    }

    public static MeshData GenerateMesh(Vector3Int position)
    {
        if (meshGeneratorShader == null)
        {
            InitializeShader();
        }

        int totalVoxelsInChunk = WorldSettings.TotalVoxelInChunk();
        int totalVoxelInChunkWithNeighbours = WorldSettings.TotalVoxelInChunkWithNeighbours();
        int kernel = meshGeneratorShader.FindKernel("GenerateChunkMesh");


        // Create buffers
        chunkVoxelsBuffer = new ComputeBuffer(totalVoxelInChunkWithNeighbours, Marshal.SizeOf(typeof(VoxelStruct)));

        trianglesBuffer = new ComputeBuffer(12 * totalVoxelInChunkWithNeighbours, Triangle.SizeOf, ComputeBufferType.Append);
        trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        // Fill buffers with data
        chunkVoxelsBuffer.SetData(VoxelManager.TransformToStruct(GetNeighbourVoxels(position)));

        // Set data in Shader
        meshGeneratorShader.SetBuffer(kernel, "chunkVoxelsBuffer", chunkVoxelsBuffer);

        meshGeneratorShader.SetBuffer(kernel, "trianglesBuffer", trianglesBuffer);
        meshGeneratorShader.SetBuffer(kernel, "trianglesCountBuffer", trianglesCountBuffer);

        WorldSettings.SetWorldSettings(meshGeneratorShader);

        trianglesBuffer.SetCounterValue(0);
        trianglesCountBuffer.SetData(new int[] { 0 });

        // Dispatch shader
        int threadGroups = WorldSettings.ThreadGroups();
        meshGeneratorShader.Dispatch(kernel, threadGroups, threadGroups, threadGroups);

        // Retrieve data
        Triangle[] triangles = new Triangle[ReadTriangleCount()];
        trianglesBuffer.GetData(triangles);

        // Release buffers
        chunkVoxelsBuffer.Release();
        trianglesBuffer.Release();
        trianglesCountBuffer.Release();

        return GenerateMeshData(triangles);
    }

    private static MeshData GenerateMeshData(Triangle[] triangles)
    {
        MeshData meshData = new MeshData();

        for (int i = 0; i < triangles.Length; i++)
        {
            int startIndex = i * 3;
            meshData.vertices.Add(triangles[i].a);
            meshData.vertices.Add(triangles[i].b);
            meshData.vertices.Add(triangles[i].c);

            meshData.triangles.Add(startIndex);
            meshData.triangles.Add(startIndex + 1);
            meshData.triangles.Add(startIndex + 2);

            meshData.uvs.Add(triangles[i].uvA);
            meshData.uvs.Add(triangles[i].uvB);
            meshData.uvs.Add(triangles[i].uvC);
        }

        return meshData;
    }

    private static int ReadTriangleCount()
    {
        int[] triCount = { 0 };
        ComputeBuffer.CopyCount(trianglesBuffer, trianglesCountBuffer, 0);
        trianglesCountBuffer.GetData(triCount);
        return triCount[0];
    }

    public static Voxel[] GetNeighbourVoxels(Vector3Int position)
    {
        int totalVoxelInChunkWithNeighbours = WorldSettings.TotalVoxelInChunkWithNeighbours();
        int totalVoxelsInAxis = WorldSettings.TotalVoxelsInAxis();


        Voxel[] voxelsWithNeighbours = new Voxel[totalVoxelInChunkWithNeighbours];

        for (int x = -1; x < totalVoxelsInAxis + 1; x++)
        {
            for (int y = -1; y < totalVoxelsInAxis + 1; y++)
            {
                for (int z = -1; z < totalVoxelsInAxis + 1; z++)
                {
                    voxelsWithNeighbours[WorldSettings.IndexFromCoordWithNeighbours(x, y, z)] =
                        GetVoxelFromChunkPosition(position, x, y, z);
                }
            }
        }

        return voxelsWithNeighbours;
    }

    public static Voxel GetVoxelFromChunkPosition(Vector3Int position, int x, int y, int z)
    {
        int totalVoxelsInAxis = WorldSettings.TotalVoxelsInAxis();

        Vector3Int neighbourChunkPosition = position;

        if (x < 0)
        {
            neighbourChunkPosition.x -= 1 * WorldSettings.feetInChunk;
            x += totalVoxelsInAxis;
        }
        else if (x >= totalVoxelsInAxis)
        {
            neighbourChunkPosition.x += 1 * WorldSettings.feetInChunk;
            x -= totalVoxelsInAxis;
        }

        if (y < 0)
        {
            neighbourChunkPosition.y -= 1 * WorldSettings.feetInChunk;
            y += totalVoxelsInAxis;
        }
        else if (y >= totalVoxelsInAxis)
        {
            neighbourChunkPosition.y += 1 * WorldSettings.feetInChunk;
            y -= totalVoxelsInAxis;
        }

        if (z < 0)
        {
            neighbourChunkPosition.z -= 1 * WorldSettings.feetInChunk;
            z += totalVoxelsInAxis;
        }
        else if (z >= totalVoxelsInAxis)
        {
            neighbourChunkPosition.z += 1 * WorldSettings.feetInChunk;
            z -= totalVoxelsInAxis;
        }

        return World.worldData.chunkDataDictionary[World.ChunkPositionFromCoords(neighbourChunkPosition)].voxels[WorldSettings.IndexFromCoord(x, y, z)];
    }
}
