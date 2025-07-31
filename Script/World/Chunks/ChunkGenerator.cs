using System.Runtime.InteropServices;
using UnityEngine;

public static class ChunkGenerator
{
    public static ComputeShader chunkGeneratorShader;
    public static ComputeBuffer chunkVoxelsBuffer;

    public static ComputeBuffer voxelsBuffer;

    public static ComputeBuffer biomeBuffer;
    public static ComputeBuffer biomeVoxelBuffer;

    private static void InitializeShader()
    {
        chunkGeneratorShader = Resources.Load<ComputeShader>("Shaders/World/chunkGeneratorShader");
    }

    public static Voxel[] GenerateChunk(Vector3Int worldPosition)
    {
        if (chunkGeneratorShader == null)
        {
            InitializeShader();
        }

        int totalVoxelsInChunk = WorldSettings.TotalVoxelInChunk();
        int kernel = chunkGeneratorShader.FindKernel("GenerateChunk");


        // Create buffers
        chunkVoxelsBuffer = new ComputeBuffer(totalVoxelsInChunk, Marshal.SizeOf(typeof(VoxelStruct)));

        VoxelStruct[] voxelStructs = VoxelManager.TransformToStruct(VoxelManager.voxels);
        chunkGeneratorShader.SetInt("voxelBufferSize", voxelStructs.Length);
        voxelsBuffer = new ComputeBuffer(voxelStructs.Length, Marshal.SizeOf(typeof(VoxelStruct)));

        BiomeStruct[] biomeStructs = BiomeManager.TransfromToStruct(BiomeManager.biomes);
        chunkGeneratorShader.SetInt("biomeBufferSize", biomeStructs.Length);
        biomeBuffer = new ComputeBuffer(biomeStructs.Length, Marshal.SizeOf(typeof(BiomeStruct)));

        BiomeVoxelStruct[] biomeVoxelStruct = BiomeManager.GetBiomeVoxelStructs(BiomeManager.biomes);
        chunkGeneratorShader.SetInt("biomeVoxelBufferSize", biomeVoxelStruct.Length);
        biomeVoxelBuffer = new ComputeBuffer(biomeVoxelStruct.Length, Marshal.SizeOf(typeof(BiomeVoxelStruct)));

        // Fill Buffers with data
        voxelsBuffer.SetData(voxelStructs);
        biomeBuffer.SetData(biomeStructs);
        biomeVoxelBuffer.SetData(biomeVoxelStruct);


        // Set data in Shader
        Vector4 worldPosition4 = new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 0) * WorldSettings.feetInChunk;
        chunkGeneratorShader.SetVector("worldPosition", worldPosition4);

        chunkGeneratorShader.SetBuffer(kernel, "voxelsBuffer", voxelsBuffer);
        chunkGeneratorShader.SetBuffer(kernel, "chunkVoxelsBuffer", chunkVoxelsBuffer);
        chunkGeneratorShader.SetBuffer(kernel, "biomeBuffer", biomeBuffer);
        chunkGeneratorShader.SetBuffer(kernel, "biomeVoxelBuffer", biomeVoxelBuffer);

        WorldSettings.SetWorldSettings(chunkGeneratorShader);


        // Dispatch shader
        int threadGroups = WorldSettings.ThreadGroups();
        chunkGeneratorShader.Dispatch(kernel, threadGroups, threadGroups, threadGroups);

        // Retrieve data
        VoxelStruct[] chunkVoxelsStructs = new VoxelStruct[totalVoxelsInChunk];

        chunkVoxelsBuffer.GetData(chunkVoxelsStructs);

        // Release buffers
        chunkVoxelsBuffer.Release();
        voxelsBuffer.Release();
        biomeBuffer.Release();
        biomeVoxelBuffer.Release();

        return VoxelManager.TransformToVoxel(chunkVoxelsStructs);
    }
}