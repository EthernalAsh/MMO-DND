using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoxelSO", menuName = "World/VoxelSO")]
public class VoxelSO : ScriptableObject
{
    public int voxelID;
    public string voxelName;
    public bool solid;

    public Texture2D topTexture;
    public Texture2D sideTexture;

    public Voxel GetVoxel()
    {
        return new Voxel(this);
    }
}

[Serializable]
public class Voxel
{
    public int voxelID;
    public string voxelName;
    public int isSolid;

    public Texture2D topTexture;
    public Texture2D sideTexture;

    public Voxel(VoxelSO voxelSO)
    {
        this.voxelID = voxelSO.voxelID;
        this.voxelName = voxelSO.voxelName;
        this.isSolid = voxelSO.solid ? 1 : 0;
        this.topTexture = voxelSO.topTexture;
        this.sideTexture = voxelSO.sideTexture;
    }

    public VoxelStruct GetVoxelStruct()
    {
        return new VoxelStruct(this);
    }
}

[Serializable]
public struct VoxelStruct
{
    public int voxelID;
    public int isSolid;

    public VoxelStruct(Voxel voxel)
    {
        this.voxelID = voxel.voxelID;
        this.isSolid = voxel.isSolid;
    }
}
