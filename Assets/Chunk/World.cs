using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static Dictionary<string, Chunk> Chunks;

    public static string BuildChunkName(Vector3 position)
    {
        return (int)position.x + "_" + (int)position.y + "_" + (int)position.z;
    }

    private IEnumerator BuildChunkColumn()
    {
        for(var index = 0; index < ColumnHeight; index++)
        {
            var chunkPosition = new Vector3(this.transform.position.x, index * ChunkSize, this.transform.position.z);
            var chunk = new Chunk(chunkPosition, TextureAtlas);
            chunk.ChunkObject.transform.parent = this.transform;
            if(!Chunks.ContainsKey(chunk.ChunkObject.name))
            {
                Chunks.Add(chunk.ChunkObject.name, chunk);
            }
        }

        foreach(var chunk in Chunks)
        {
            chunk.Value.DrawChunk();
            yield return null;
        }
    }

    private void Start()
    {
        Chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildChunkColumn());
    }
}
