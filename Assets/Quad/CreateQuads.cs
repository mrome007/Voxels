﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour 
{
    public enum CubeSide
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
        STONE
    }

    public Material CubeMaterial;
    public BlockType BType;

    private Vector2[,] blockUVs = 
    { 
        /*GRASS TOP*/       {new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
            new Vector2( 0.125f, 0.4375f ),new Vector2( 0.1875f, 0.4375f )},
        /*GRASS SIDE*/      {new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
            new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
        /*DIRT*/            {new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
            new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*STONE*/           {new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
            new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )}
    }; 

    private void CreateQuad(CubeSide side)
    {
        var mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        var vertices = new Vector3[4]; //cube has 4 vertices.
        var normals = new Vector3[4]; //the 4 vertices have a normal respectively.
        var uvs = new Vector2[4]; //texture mapping for each vertex.
        var triangles = new int[6]; //6 triangles in a cube.

        //all possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if(BType == BlockType.GRASS && side == CubeSide.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if(BType == BlockType.GRASS && side == CubeSide.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(BType + 1), 0];
            uv10 = blockUVs[(int)(BType + 1), 1];
            uv01 = blockUVs[(int)(BType + 1), 2];
            uv11 = blockUVs[(int)(BType + 1), 3];
        }

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
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        var quad = new GameObject("quad");
        quad.transform.parent = this.gameObject.transform;
        var meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
        var renderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = CubeMaterial;
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

        foreach(Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }

    private void CreateCube()
    {
        CreateQuad(CubeSide.FRONT);
        CreateQuad(CubeSide.BACK);
        CreateQuad(CubeSide.LEFT);
        CreateQuad(CubeSide.RIGHT);
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.BOTTOM);
        CombineQuads();
    }

    private void Start()
    {
        CreateCube();
    }
}
