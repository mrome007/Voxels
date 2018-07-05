using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material CubeMaterial;
    public Block[,,] ChunkData;
    public GameObject ChunkObject;

    private void BuildChunk()
    {
        ChunkData = new Block[World.ChunkSize, World.ChunkSize, World.ChunkSize];

        for(var z = 0; z < World.ChunkSize; z++)
        {
            for(var y = 0; y < World.ChunkSize; y++)
            {
                for(var x = 0; x < World.ChunkSize; x++)
                {
                    var pos = new Vector3(x, y, z);
                    var worldX = (int)(x + ChunkObject.transform.position.x);
                    var worldY = (int)(y + ChunkObject.transform.position.y);
                    var worldZ = (int)(z + ChunkObject.transform.position.z);
                    if(worldY <= Utils.GenerateHeight(worldX, worldZ))
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, ChunkObject.gameObject, this);
                    }
                    else
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, ChunkObject.gameObject, this);
                    }

                }
            }
        }


    }

    public void DrawChunk()
    {
        for(var z = 0; z < World.ChunkSize; z++)
        {
            for(var y = 0; y < World.ChunkSize; y++)
            {
                for(var x = 0; x < World.ChunkSize; x++)
                {
                    ChunkData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
    }

    public Chunk(Vector3 position, Material mat)
    {
        ChunkObject = new GameObject(World.BuildChunkName(position));
        ChunkObject.transform.position = position;
        CubeMaterial = mat;
        BuildChunk();
    }

    private void CombineQuads()
    {
        var meshFilters = ChunkObject.GetComponentsInChildren<MeshFilter>();
        var combine = new CombineInstance[meshFilters.Length];

        var i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        var mf = (MeshFilter)ChunkObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);

        var renderer = ChunkObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = CubeMaterial;

        foreach(Transform quad in ChunkObject.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
