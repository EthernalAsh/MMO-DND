using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;

    public bool showGizmos = false;

    public ChunkData ChunkData { get; private set; }
    public VoxelStruct[] voxels;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    public void InitializeChunk(ChunkData chunkData)
    {
        this.ChunkData = chunkData;
        voxels = VoxelManager.TransformToStruct(chunkData.voxels);
    }

    private void RenderMesh(MeshData meshData)
    {
        mesh.Clear();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = meshData.vertices.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.SetUVs(0, meshData.uvs);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

    public void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        for (int i = 0; i < ChunkData.voxels.Length; i++)
        {

            if (ChunkData.voxels[i].voxelID != 0)
            {
                Vector3 voxelPosition = WorldSettings.CoordFromIndex(i) * WorldSettings.voxelSize + ChunkData.worldPosition;

                Color color = Color.black;

                if (ChunkData.voxels[i].voxelID == 1) color = Color.white;
                if (ChunkData.voxels[i].voxelID == 21) color = Color.red;
                if (ChunkData.voxels[i].voxelID == 22) color = Color.yellow;
                if (ChunkData.voxels[i].voxelID == 23) color = Color.blue;

                Gizmos.color = color;
                Gizmos.DrawCube(voxelPosition, new Vector3(WorldSettings.voxelSize, WorldSettings.voxelSize, WorldSettings.voxelSize) * 0.75f);
            }
        }
    }
}