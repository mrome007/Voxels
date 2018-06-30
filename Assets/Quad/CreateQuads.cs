using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour 
{
    public Material CubeMaterial;

    private void CreateQuad()
    {
        var mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        var vertices = new Vector3[4]; //cube has 4 vertices.
        var normals = new Vector3[4]; //the 4 vertices have a normal respectively.
        var uvs = new Vector2[4]; //texture mapping for each vertex.
        var triangles = new int[6]; //6 triangles in a cube.

        //all possible UVs
        var uv00 = new Vector2(0f, 0f);
        var uv01 = new Vector2(0f, 1f);
        var uv10 = new Vector2(1f, 0f);
        var uv11 = new Vector2(1f, 1f);

        //all possible vertices
        var p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        var p1 = new Vector3(0.5f, -0.5f, 0.5f);
        var p2 = new Vector3(0.5f, -0.5f, -0.5f);
        var p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        var p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        var p5 = new Vector3(0.5f, 0.5f, 0.5f);
        var p6 = new Vector3(0.5f, 0.5f, -0.5f);
        var p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        vertices = new Vector3[] { p4, p5, p1, p0 };
        normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
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

    private void Start()
    {
        CreateQuad();
    }
}
