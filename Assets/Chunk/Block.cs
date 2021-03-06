﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    private enum CubeSide
    {
        BOTTOM,
        TOP,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    }

    public enum BlockType
    {
        GRASS,
        DIRT,
        STONE,
        DIAMOND,
        REDSTONE,
        BEDROCK,
        NOCRACK,
        CRACK1,
        CRACK2,
        CRACK3,
        CRACK4,
        AIR
    }



    Vector2[,] blockUVs = 
    { 
        /*GRASS TOP*/       {new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
            new Vector2( 0.125f, 0.4375f ),new Vector2( 0.1875f, 0.4375f )},
        /*GRASS SIDE*/      {new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
            new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
        /*DIRT*/            {new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
            new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*STONE*/           {new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
            new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},
        /*BEDROCK*/         {new Vector2( 0.3125f, 0.8125f ), new Vector2( 0.375f, 0.8125f),
            new Vector2( 0.3125f, 0.875f ),new Vector2( 0.375f, 0.875f )},
        /*REDSTONE*/        {new Vector2( 0.1875f, 0.75f ), new Vector2( 0.25f, 0.75f),
            new Vector2( 0.1875f, 0.8125f ),new Vector2( 0.25f, 0.8125f )},
        /*DIAMOND*/         {new Vector2( 0.125f, 0.75f ), new Vector2( 0.1875f, 0.75f),
            new Vector2( 0.125f, 0.8125f ),new Vector2( 0.1875f, 0.8125f )},
        /*NOCRACK*/         {new Vector2( 0.6875f, 0f ), new Vector2( 0.75f, 0f),
            new Vector2( 0.6875f, 0.0625f ),new Vector2( 0.75f, 0.0625f )},
        /*CRACK1*/          { new Vector2(0f,0f),  new Vector2(0.0625f,0f),
            new Vector2(0f,0.0625f), new Vector2(0.0625f,0.0625f)},
        /*CRACK2*/          { new Vector2(0.0625f,0f),  new Vector2(0.125f,0f),
            new Vector2(0.0625f,0.0625f), new Vector2(0.125f,0.0625f)},
        /*CRACK3*/          { new Vector2(0.125f,0f),  new Vector2(0.1875f,0f),
            new Vector2(0.125f,0.0625f), new Vector2(0.1875f,0.0625f)},
        /*CRACK4*/          { new Vector2(0.1875f,0f),  new Vector2(0.25f,0f),
            new Vector2(0.1875f,0.0625f), new Vector2(0.25f,0.0625f)}
    }; 

    public bool IsSolid;

    public BlockType bType;
    private GameObject parent;
    private Vector3 position;
    private Chunk owner;

    public BlockType Health;
    private int currentHealth;
    private int[] blockHealthMax = { 3, 3, 4, 4, 4, -1, 0, 0, 0, 0, 0, 0 };

    public Block(BlockType blk, Vector3 pos, GameObject par, Chunk own)
    {
        bType = blk;
        owner = own;
        position = pos;
        parent = par;
        IsSolid = bType != BlockType.AIR;
        Health = BlockType.NOCRACK;
        currentHealth = blockHealthMax[(int)bType];
    }

    public void SetType(BlockType type)
    {
        bType = type;
        IsSolid = bType != BlockType.AIR;

        Health = BlockType.NOCRACK;
        currentHealth = blockHealthMax[(int)bType];
    }

    public bool HitBlock()
    {
        if(currentHealth == -1)
        {
            return false;
        }
        currentHealth--;
        Health++;
        if(currentHealth <= 0)
        {
            bType = BlockType.AIR;
            IsSolid = false;
            Health = BlockType.NOCRACK;
            owner.Redraw();
            return true;
        }

        owner.Redraw();
        return false;
    }

    private void CreateQuad(CubeSide side)
    {
        var mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        var vertices = new Vector3[4]; //cube has 4 vertices.
        var normals = new Vector3[4]; //the 4 vertices have a normal respectively.
        var uvs = new Vector2[4]; //texture mapping for each vertex.
        var suvs = new List<Vector2>();
        var triangles = new int[6]; //6 triangles in a cube.

        //all possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if(bType == BlockType.GRASS && side == CubeSide.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if(bType == BlockType.GRASS && side == CubeSide.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }

        suvs.Add(blockUVs[(int)(Health + 1), 3]);
        suvs.Add(blockUVs[(int)(Health + 1), 2]);
        suvs.Add(blockUVs[(int)(Health + 1), 0]);
        suvs.Add(blockUVs[(int)(Health + 1), 1]);

        //all possible vertices
        var p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        var p1 = new Vector3(0.5f, -0.5f, 0.5f);
        var p2 = new Vector3(0.5f, -0.5f, -0.5f);
        var p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        var p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        var p5 = new Vector3(0.5f, 0.5f, 0.5f);
        var p6 = new Vector3(0.5f, 0.5f, -0.5f);
        var p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        //SET UP VERTICES IN CLOCKWISE DIRECTION TO FACE FORWARD.
        switch(side)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;

            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;

            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;

            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;

            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;

            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;

            default:
                break;
        }

        uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.SetUVs(1, suvs);
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        var quad = new GameObject("Quad");
        quad.transform.position = position;
        quad.transform.parent = parent.transform;

        var meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
    }

    public void Draw()
    {
        if(bType == BlockType.AIR)
        {
            return;
        }
        
        if(!HasSolidNeighbor((int)position.x, (int)position.y, (int)position.z + 1))
        {
            CreateQuad(CubeSide.FRONT);
        }
        if(!HasSolidNeighbor((int)position.x, (int)position.y, (int)position.z - 1))
        {
            CreateQuad(CubeSide.BACK);
        }
        if(!HasSolidNeighbor((int)position.x - 1, (int)position.y, (int)position.z))
        {
            CreateQuad(CubeSide.LEFT);
        }
        if(!HasSolidNeighbor((int)position.x + 1, (int)position.y, (int)position.z))
        {
            CreateQuad(CubeSide.RIGHT);
        }
        if(!HasSolidNeighbor((int)position.x, (int)position.y + 1, (int)position.z))
        {
            CreateQuad(CubeSide.TOP);
        }
        if(!HasSolidNeighbor((int)position.x, (int)position.y - 1, (int)position.z))
        {
            CreateQuad(CubeSide.BOTTOM);
        }
    }

    private int ConvertBlockIndexToLocal(int i)
    {
        if(i == -1)
        {
            i = World.ChunkSize - 1;
        }
        else if(i == World.ChunkSize)
        {
            i = 0;
        }

        return i;
    }

    public bool HasSolidNeighbor(int x, int y, int z)
    {
        Block[,,] chunks;

        //Check if neighbor is in another chunk
        if(x < 0 || x >= World.ChunkSize || y < 0 || y >= World.ChunkSize || z < 0 || z >= World.ChunkSize)
        {
            var neightborChunkPos = this.parent.transform.position +
                                    new Vector3((x - (int)position.x) * World.ChunkSize, (y - (int)position.y) * World.ChunkSize, (z - (int)position.z) * World.ChunkSize);

            var nName = World.BuildChunkName(neightborChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk nChunk;
            if(World.Chunks.TryGetValue(nName, out nChunk))
            {
                chunks = nChunk.ChunkData;
            }
            else
            {
                return false;
            }
        }
        else
        {
            chunks = owner.ChunkData;
        }

        try
        {
            return chunks[x, y, z].IsSolid;
        }
        catch(Exception ex)
        {
        }

        return false;
    }
}
