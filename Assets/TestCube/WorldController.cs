using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour 
{
    public GameObject Block;
    public int width = 10;
    public int height = 2;
    public int depth = 10;

    public IEnumerator BuildWorld()
    {
        for(int z = 0; z < depth; z++)
        {
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    var pos = new Vector3(x, y, z);
                    var cube = GameObject.Instantiate(Block, pos, Quaternion.identity);
                    cube.name = x + "_" + y + "_" + z;
                }
                yield return null;
            }
        }
    }

    private void Start()
    {
        StartCoroutine(BuildWorld());
    }
}
