using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour 
{
    [SerializeField]
    private int xWorld = 4;

    [SerializeField]
    private int yWorld = 4;

    [SerializeField]
    private int zWorld = 4;

    public Material CubeMaterial;
    public Block[,,] ChunkData;

    private IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
    {
        ChunkData = new Block[sizeX, sizeY, sizeZ];

        for(var z = 0; z < sizeZ; z++)
        {
            for(var y = 0; y < sizeY; y++)
            {
                for(var x = 0; x < sizeX; x++)
                {
                    var pos = new Vector3(x, y, z);
                    if(Random.Range(0, 100) < 50)
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, this.gameObject, CubeMaterial);
                    }
                    else
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, this.gameObject, CubeMaterial);
                    }

                }
            }
        }

        for(var z = 0; z < sizeZ; z++)
        {
            for(var y = 0; y < sizeY; y++)
            {
                for(var x = 0; x < sizeX; x++)
                {
                    ChunkData[x, y, z].Draw();
                }
            }
        }

        CombineQuads();
        yield return null;
    }

    private void Start()
    {
        StartCoroutine(BuildChunk(xWorld, yWorld, zWorld));
    }

    private void CombineQuads()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        var combine = new CombineInstance[meshFilters.Length];

        var i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        var mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);

        var renderer = this.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = CubeMaterial;

        foreach(Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }
}
