using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeSO", menuName = "World/BiomeSO")]
public class BiomeSO : ScriptableObject
{
    public int biomeID;
    public string biomeName;

    [Space]
    [Header("BiomeSettings")]
    [Range(0, 1f)] public float tempeture;
    [Range(0, 1f)] public float humidity;

    [Space]
    [Header("Noise Settings")]
    public float noiseScale;
    public float frequency;
    public int octaves;
    public float amplitude;


    public BiomeVoxel[] biomeVoxels;

    public Biome GetBiome()
    {
        return new Biome(this);
    }
}

public class Biome
{
    public int biomeID;
    public string biomeName;

    public float tempeture;
    public float humidity;

    public float noiseScale;
    public float frequency;
    public int octaves;
    public float amplitude;

    public BiomeVoxel[] biomeVoxels;

    public Biome(BiomeSO biomeSO)
    {
        this.biomeID = biomeSO.biomeID;
        this.biomeName = biomeSO.biomeName;
        this.biomeVoxels = biomeSO.biomeVoxels;

        this.tempeture = biomeSO.tempeture;
        this.humidity = biomeSO.humidity;

        this.noiseScale = biomeSO.noiseScale;
        this.frequency = biomeSO.frequency;
        this.octaves = biomeSO.octaves;
        this.amplitude = biomeSO.amplitude;
    }

    public BiomeStruct GetBiomeStruct()
    {
        return new BiomeStruct(this);
    }

    public BiomeVoxelStruct[] GetBiomeVoxelStructs()
    {
        BiomeVoxelStruct[] biomeVoxelStructs = new BiomeVoxelStruct[biomeVoxels.Length];
        for (int i = 0; i < biomeVoxels.Length; i++)
        {
            biomeVoxelStructs[i] = biomeVoxels[i].GetBiomeVoxelStruct(biomeID);
        }
        return biomeVoxelStructs;
    }
}

public struct BiomeStruct
{
    public int biomeID;

    public float tempeture;
    public float humidity;

    public float noiseScale;
    public float frequency;
    public int octaves;
    public float amplitude;

    public BiomeStruct(Biome biome)
    {
        this.biomeID = biome.biomeID;

        this.tempeture = biome.tempeture;
        this.humidity = biome.humidity;

        this.noiseScale = biome.noiseScale;
        this.frequency = biome.frequency;
        this.octaves = biome.octaves;
        this.amplitude = biome.amplitude;
    }
}

[Serializable]
public class BiomeVoxel
{
    public VoxelSO voxelSO;
    public int voxelDepth;

    public BiomeVoxelStruct GetBiomeVoxelStruct(int biomeID)
    {
        return new BiomeVoxelStruct(biomeID, this);
    }
}


public struct BiomeVoxelStruct
{
    public int biomeID;
    public int voxelID;
    public int voxelDepth;

    public BiomeVoxelStruct(int biomeID, BiomeVoxel biomeVoxel)
    {
        this.biomeID = biomeID;
        this.voxelID = biomeVoxel.voxelSO.voxelID;
        this.voxelDepth = biomeVoxel.voxelDepth;
    }
}