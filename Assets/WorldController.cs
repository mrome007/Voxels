using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour 
{
    public GameObject Block;
    public int WorldSize = 2;

    public IEnumerator BuildWorld()
    {
        for(int z = 0; z < WorldSize; z++)
        {
            for(int y = 0; y < WorldSize; y++)
            {
                for(int x = 0; x < WorldSize; x++)
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
