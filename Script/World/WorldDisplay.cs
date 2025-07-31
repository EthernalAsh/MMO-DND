using UnityEngine;

public class WorldDisplay : MonoBehaviour
{
    private void OnValidate()
    {
        VoxelManager.InitializeVoxels();
        BiomeManager.InitializeBiomes();
        SetVoxels();
    }

    public Voxel[] voxels;
    public void SetVoxels()
    {
        voxels = ChunkGenerator.GenerateChunk(new Vector3Int(0, 0, 0));
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < voxels.Length; i++)
        {

            if (voxels[i].voxelID != 0)
            {
                Vector3 voxelPosition = WorldSettings.CoordFromIndex(i) * WorldSettings.voxelSize;

                Color color = Color.black;

                if (voxels[i].voxelID == 1) color = Color.white;
                if (voxels[i].voxelID == 21) color = Color.red;
                if (voxels[i].voxelID == 22) color = Color.yellow;
                if (voxels[i].voxelID == 23) color = Color.blue;

                Gizmos.color = color;
                Gizmos.DrawCube(voxelPosition, new Vector3(WorldSettings.voxelSize, WorldSettings.voxelSize, WorldSettings.voxelSize) * 0.75f);
            }
        }
    }
}