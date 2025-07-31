using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;

    public static WorldData worldData;

    public GameObject chunkPrefab;

    public Material worldMaterial;

    private void Awake()
    {
        instance = this;
        worldMaterial.SetTexture("_BaseTextures", VoxelManager.GetTextures());
        worldData = new WorldData
        {
            chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
            chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
        };
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        GenerateWorld(Vector3Int.zero);
    }

    WorldGenerationData worldGenerationData;

    public void GenerateWorld(Vector3Int position)
    {
        worldGenerationData = GetPositionInRenderDistance(position);

        foreach (Vector3Int pos in worldGenerationData.chunkPositionsToRemove)
        {
            RemoveChunk(pos);
        }

        foreach (Vector3Int pos in worldGenerationData.chunkDataPositionsToRemove)
        {
            RemoveChunkData(pos);
        }

        GenerateChunkData();

        //foreach (Vector3Int pos in worldGenerationData.chunkPositionsToCreate)
        //{
        //    ChunkData chunkData = worldData.chunkDataDictionary[pos];
        //    MeshData meshData = MeshGenerator.GenerateMesh(pos);
        //    GameObject chunkObject = Instantiate(chunkPrefab, chunkData.worldPosition, Quaternion.identity);

        //    ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        //    chunkRenderer.transform.SetParent(this.transform);
        //    worldData.chunkDictionary.Add(chunkData.worldPosition, chunkRenderer);

        //    chunkRenderer.InitializeChunk(chunkData);
        //    chunkRenderer.UpdateChunk(meshData);
        //}
    }

    private void Update()
    {
        GenerateChunk();
    }

    private void GenerateChunkData()
    {
        foreach (Vector3Int pos in worldGenerationData.chunkDataPositionsToCreate)
        {
            ChunkData chunkData = new ChunkData(this, pos);
            chunkData.voxels = ChunkGenerator.GenerateChunk(pos);
            worldData.chunkDataDictionary.Add(pos, chunkData);
        }
    }

    private void GenerateChunk()
    {
        if (worldGenerationData.chunkPositionsToCreate.Count > 0)
        {
            foreach (Vector3Int pos in worldGenerationData.chunkPositionsToCreate)
            {
                ChunkData chunkData = worldData.chunkDataDictionary[pos];
                MeshData meshData = MeshGenerator.GenerateMesh(pos);
                GameObject chunkObject = Instantiate(chunkPrefab, chunkData.worldPosition, Quaternion.identity);

                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                chunkRenderer.transform.SetParent(this.transform);
                worldData.chunkDictionary.Add(chunkData.worldPosition, chunkRenderer);

                chunkRenderer.InitializeChunk(chunkData);
                chunkRenderer.UpdateChunk(meshData);
                worldGenerationData.chunkPositionsToCreate.Remove(pos);
                return;
            }
        }
    }

    private WorldGenerationData GetPositionInRenderDistance(Vector3Int position)
    {
        List<Vector3Int> chunkPositionsNeeded = GetChunkPositions(position);
        List<Vector3Int> chunkDataPositionsNeeded = GetChunkPositions(position, true);

        List<Vector3Int> chunkPositionsToCreate = SelectPositionsToCreate(chunkPositionsNeeded, position);
        List<Vector3Int> chunkDataPositionsToCreate = SelectDataPositionsToCreate(chunkDataPositionsNeeded, position);

        List<Vector3Int> chunkPositionsToRemove = GetUnneededChunks(worldData, chunkPositionsNeeded);
        List<Vector3Int> chunkDataPositionsToRemove = GetUnneededData(worldData, chunkDataPositionsNeeded);

        WorldGenerationData worldGenerationData = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataPositionsToRemove = chunkDataPositionsToRemove
        };
        return worldGenerationData;
    }

    public static Vector3Int ChunkPositionFromCoords(Vector3Int position)
    {
        int feetInChunk = WorldSettings.feetInChunk;
        return new Vector3Int
        {
            x = Mathf.FloorToInt(position.x / (float)feetInChunk) * feetInChunk,
            y = Mathf.FloorToInt(position.y / (float)feetInChunk) * feetInChunk,
            z = Mathf.FloorToInt(position.z / (float)feetInChunk) * feetInChunk
        };
    }

    private static List<Vector3Int> GetChunkPositions(Vector3Int position, bool addBorder = false)
    {
        int feetInChunk = WorldSettings.feetInChunk;
        int renderDistance = WorldSettings.GetRenderDistance() + (addBorder ? 1 : 0);

        int startX = position.x - (renderDistance) * feetInChunk;
        int startY = position.y - (renderDistance) * feetInChunk;
        int startZ = position.z - (renderDistance) * feetInChunk;

        int endX = position.x + (renderDistance) * feetInChunk;
        int endY = position.y + (renderDistance) * feetInChunk;
        int endZ = position.z + (renderDistance) * feetInChunk;

        List<Vector3Int> chunkPositions = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += feetInChunk)
        {
            for (int y = startY; y <= endY; y += feetInChunk)
            {
                for (int z = startZ; z <= endZ; z += feetInChunk)
                {
                    Vector3Int chunkPosition = ChunkPositionFromCoords(new Vector3Int(x, y, z));
                    chunkPositions.Add(chunkPosition);
                }
            }
        }

        return chunkPositions;
    }

    private static List<Vector3Int> SelectPositionsToCreate(List<Vector3Int> chunkPositionsNeeded, Vector3Int position)
    {
        return chunkPositionsNeeded
            .Where(pos => worldData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(position, pos))
            .ToList();
    }

    private static List<Vector3Int> SelectDataPositionsToCreate(List<Vector3Int> chunkDataPositionsNeeded, Vector3Int positions)
    {
        return chunkDataPositionsNeeded
            .Where(pos => worldData.chunkDataDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(positions, pos))
            .ToList();
    }

    private static List<Vector3Int> GetUnneededData(WorldData worldData, List<Vector3Int> chunkDataPositionsNeeded)
    {
        return worldData.chunkDataDictionary.Keys
            .Where(pos => chunkDataPositionsNeeded.Contains(pos) == false)
            .ToList();
    }

    private static List<Vector3Int> GetUnneededChunks(WorldData worldData, List<Vector3Int> chunkPositionsNeeded)
    {
        List<Vector3Int> positionsToRemove = new List<Vector3Int>();
        foreach (var pos in worldData.chunkDictionary.Keys
            .Where(pos => chunkPositionsNeeded.Contains(pos) == false))
        {
            if (worldData.chunkDictionary.ContainsKey(pos))
            {
                positionsToRemove.Add(pos);
            }
        }

        return positionsToRemove;
    }

    private static void RemoveChunkData(Vector3Int pos)
    {
        worldData.chunkDataDictionary.Remove(pos);
    }

    private static void RemoveChunk(Vector3Int pos)
    {
        ChunkRenderer chunk = null;
        if (worldData.chunkDictionary.TryGetValue(pos, out chunk))
        {
            RemoveChunk(chunk);
            worldData.chunkDictionary.Remove(pos);
        }
    }

    private static void RemoveChunk(ChunkRenderer chunk)
    {
        chunk.gameObject.SetActive(false);
    }
}

public struct WorldGenerationData
{
    public List<Vector3Int> chunkPositionsToCreate;
    public List<Vector3Int> chunkDataPositionsToCreate;
    public List<Vector3Int> chunkPositionsToRemove;
    public List<Vector3Int> chunkDataPositionsToRemove;
}

public struct WorldData
{
    public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
    public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
}