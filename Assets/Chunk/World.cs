using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;


public class World : MonoBehaviour {

    public GameObject Player;
    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static int WorldSize = 1;
    public static int Radius = 4;
    public static ConcurrentDictionary<string, Chunk> Chunks;
    public static bool Firstbuild = true;

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

            yield return null;
        }
    }

    // Use this for initialization
    private void Start() 
    {
        var ppos = Player.transform.position;
        Player.transform.position = new Vector3(ppos.x,
            Utils.GenerateHeight(ppos.x,ppos.z) + 1,
            ppos.z);

        Player.SetActive(false);

        Firstbuild = true;
        Chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;  

        //build starting chunk
        BuildChunkAt((int)(Player.transform.position.x/ChunkSize),
            (int)(Player.transform.position.y/ChunkSize),
            (int)(Player.transform.position.z/ChunkSize));
        //draw it
        StartCoroutine(DrawChunks());

        //create a bigger world
        StartCoroutine(BuildRecursiveWorld((int)(Player.transform.position.x/ChunkSize),
            (int)(Player.transform.position.y/ChunkSize),
            (int)(Player.transform.position.z/ChunkSize),Radius));
    }

    // Update is called once per frame
    private void Update () 
    {
        if(!Player.activeSelf)
        {
            Player.SetActive(true); 
            Firstbuild = false;
        }
    }
}
