using UnityEngine;

public static class VoxelManager
{
    public static Voxel[] voxels;
    public static Voxel emptyVoxel;

    private static int topVoxelID = 0;

    public static void InitializeVoxels()
    {
        VoxelSO[] voxelsSO = Resources.LoadAll<VoxelSO>("World/Voxels");

        voxels = new Voxel[voxelsSO.Length];
        for (int i = 0; i < voxelsSO.Length; i++)
        {
            voxels[i] = voxelsSO[i].GetVoxel();
            if (voxels[i].voxelID == 0)
            {
                emptyVoxel = voxels[i];
            }

            if (topVoxelID < voxelsSO[i].voxelID)
            {
                topVoxelID = voxelsSO[i].voxelID;
            }
        }
    }

    public static Voxel GetVoxelWithID(int id)
    {
        foreach (Voxel voxel in voxels)
        {
            if (voxel.voxelID == id)
            {
                return voxel;
            }
        }

        return emptyVoxel;
    }

    public static VoxelStruct[] TransformToStruct(Voxel[] voxels)
    {
        VoxelStruct[] voxelStructs = new VoxelStruct[voxels.Length];
        for (int i = 0; i < voxels.Length; i++)
        {
            voxelStructs[i] = voxels[i].GetVoxelStruct();
        }
        return voxelStructs;
    }

    public static Voxel[] TransformToVoxel(VoxelStruct[] voxelStructs)
    {
        Voxel[] voxels = new Voxel[voxelStructs.Length];
        for (int i = 0; i < voxelStructs.Length; i++)
        {
            voxels[i] = GetVoxelWithID(voxelStructs[i].voxelID);
        }
        return voxels;
    }

    public static Texture2DArray GetTextures()
    {
        Texture2DArray textures = new Texture2DArray(1024, 1024, (topVoxelID + 1) * 2, TextureFormat.RGBA32, false, false);

        foreach (Voxel voxel in voxels)
        {
            if (voxel.topTexture != null)
            {
                textures.SetPixels(voxel.topTexture.GetPixels(), voxel.voxelID);
            }

            if (voxel.sideTexture != null)
            {
                textures.SetPixels(voxel.sideTexture.GetPixels(), voxel.voxelID + topVoxelID);
            }
            else if (voxel.topTexture != null)
            {
                textures.SetPixels(voxel.topTexture.GetPixels(), voxel.voxelID + topVoxelID);
            }
        }

        textures.Apply();
        return textures;
    }
}