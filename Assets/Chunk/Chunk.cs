using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material CubeMaterial;
    public Block[,,] ChunkData;
    public GameObject ChunkObject;

    public enum ChunkStatus 
    {
        DRAW, 
        DONE, 
        KEEP
    }

    public ChunkStatus Status;

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

                    if(Utils.FractalBrownianMethod3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, ChunkObject.gameObject, this);
                    }
                    else if(worldY == 0)
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, ChunkObject.gameObject, this);
                    }
                    else if(worldY <= Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        if(Utils.FractalBrownianMethod3D(worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY < 40f)
                        {
                            ChunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, ChunkObject.gameObject, this);
                        }
                        else if(Utils.FractalBrownianMethod3D(worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20f)
                        {
                            ChunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos, ChunkObject.gameObject, this);
                        }
                        else
                        {
                            ChunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, ChunkObject.gameObject, this);
                        }
                    }
                    else if(worldY == Utils.GenerateHeight(worldX, worldZ))
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, ChunkObject.gameObject, this);
                    }
                    else if(worldY < Utils.GenerateHeight(worldX, worldZ))
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, ChunkObject.gameObject, this);
                    }
                    else
                    {
                        ChunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, ChunkObject.gameObject, this);
                    }

                    Status = ChunkStatus.DRAW;
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
        var collider = ChunkObject.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = ChunkObject.transform.GetComponent<MeshFilter>().mesh;
        Status = ChunkStatus.DONE;
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
