using System.Collections.Generic;
using UnityEngine;

public static class BiomeManager
{
    public static Biome[] biomes;

    public static void InitializeBiomes()
    {
        BiomeSO[] biomeSO = Resources.LoadAll<BiomeSO>("World/Biomes");

        biomes = new Biome[biomeSO.Length];
        for (int i = 0; i < biomeSO.Length; i++)
        {
            biomes[i] = biomeSO[i].GetBiome();
        }
    }

    public static Biome GetBiomeWithID(int id)
    {
        foreach (Biome biome in biomes)
        {
            if (biome.biomeID == id)
            {
                return biome;
            }
        }
        return biomes[0];
    }

    public static BiomeStruct[] TransfromToStruct(Biome[] biomes)
    {
        BiomeStruct[] biomeStructs = new BiomeStruct[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeStructs[i] = biomes[i].GetBiomeStruct();
        }
        return biomeStructs;
    }

    public static BiomeVoxelStruct[] GetBiomeVoxelStructs(Biome[] biomes)
    {
        List<BiomeVoxelStruct> biomeVoxelStructs = new List<BiomeVoxelStruct>();

        foreach (Biome biome in biomes)
        {
            foreach (BiomeVoxelStruct biomeVoxelStruct in biome.GetBiomeVoxelStructs())
            {
                biomeVoxelStructs.Add(biomeVoxelStruct);
            }
        }

        return biomeVoxelStructs.ToArray();
    }
}
