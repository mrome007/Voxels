using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;


public class World : MonoBehaviour 
{

    public GameObject Player;
    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static int WorldSize = 1;
    public static int Radius = 6;
    public static ConcurrentDictionary<string, Chunk> Chunks;
    public static bool Firstbuild = true;

    public static List<string> ToRemove = new List<string>();

    private Vector3 lastBuildPos;
    private CoroutineQueue queue;
    public static uint maxCoroutines = 1000;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + 
            (int)v.y + "_" + 
            (int)v.z;
    }

    void BuildChunkAt(int x, int y, int z)
    {
        var chunkPosition = new Vector3(x*ChunkSize, 
            y*ChunkSize, 
            z*ChunkSize);

        var n = BuildChunkName(chunkPosition);
        Chunk c;

        if(!Chunks.TryGetValue(n, out c))
        {
            c = new Chunk(chunkPosition, TextureAtlas);
            c.ChunkObject.transform.parent = this.transform;
            Chunks.TryAdd(c.ChunkObject.name, c);
        }

    }

    private IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad)
    {
        rad--;

        if(rad <= 0)
        {
            yield break;
        }

        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecursiveWorld(x, y, z - 1, rad));

        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecursiveWorld(x, y - 1, z, rad));

        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecursiveWorld(x - 1, y, z, rad));

        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecursiveWorld(x, y, z + 1, rad));

        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecursiveWorld(x, y + 1, z, rad));

        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecursiveWorld(x + 1, y, z, rad));
        yield return null;
    }

    private IEnumerator DrawChunks()
    {
        foreach(var c in Chunks)
        {
            if(c.Value.Status == Chunk.ChunkStatus.DRAW) 
            {
                c.Value.DrawChunk();
            }

            if(c.Value.ChunkObject && Vector3.Distance(Player.transform.position, c.Value.ChunkObject.transform.position) > Radius * ChunkSize)
            {
                ToRemove.Add(c.Key);
            }

            yield return null;
        }
    }

    private IEnumerator RemoveOldChunks()
    {
        for(var i = 0; i < ToRemove.Count; i++)
        {
            var name = ToRemove[i];
            Chunk chunk;
            if(Chunks.TryGetValue(name, out chunk))
            {
                Destroy(chunk.ChunkObject);
                Chunks.TryRemove(name, out chunk);
                yield return null;
            }
        }
    }

    private void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");
        queue.Run(BuildRecursiveWorld((int)(Player.transform.position.x/ChunkSize),
            (int)(Player.transform.position.y/ChunkSize),
            (int)(Player.transform.position.z/ChunkSize),Radius));
    }

    // Use this for initialization
    private void Start() 
    {
        var ppos = Player.transform.position;
        Player.transform.position = new Vector3(ppos.x,
            Utils.GenerateHeight(ppos.x,ppos.z) + 1,
            ppos.z);

        lastBuildPos = Player.transform.position;
        Player.SetActive(false);

        Firstbuild = true;
        Chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCoroutines, StartCoroutine);

        //build starting chunk
        BuildChunkAt((int)(Player.transform.position.x/ChunkSize),
            (int)(Player.transform.position.y/ChunkSize),
            (int)(Player.transform.position.z/ChunkSize));
        //draw it
        queue.Run(DrawChunks());

        //create a bigger world
        queue.Run(BuildRecursiveWorld((int)(Player.transform.position.x/ChunkSize),
            (int)(Player.transform.position.y/ChunkSize),
            (int)(Player.transform.position.z/ChunkSize),Radius));
    }

    // Update is called once per frame
    private void Update() 
    {
        var movement = lastBuildPos - Player.transform.position;
        if(movement.magnitude > ChunkSize)
        {
            lastBuildPos = Player.transform.position;
            BuildNearPlayer();
        }

        if(!Player.activeSelf)
        {
            Player.SetActive(true); 
            Firstbuild = false;
        }

        queue.Run(DrawChunks());
        queue.Run(RemoveOldChunks());
    }
}
