using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour 
{
    public GameObject Camera;

    private void Update()
    {
        BlockInteractionInput();
    }

    private void BlockInteractionInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 10))
            {
                var hitBlock = hit.point - hit.normal / 2.0f;

                var x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                var y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                var z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                var updates = new List<string>();
                var thisChunkX = hit.collider.gameObject.transform.position.x;
                var thisChunkY = hit.collider.gameObject.transform.position.y;
                var thisChunkZ = hit.collider.gameObject.transform.position.z;

                updates.Add(hit.collider.gameObject.name);

                if(x == 0)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX - World.ChunkSize, thisChunkY, thisChunkZ)));
                }

                if(x == World.ChunkSize - 1)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX + World.ChunkSize, thisChunkY, thisChunkZ)));
                }

                if(y == 0)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY - World.ChunkSize, thisChunkZ)));
                }

                if(y == World.ChunkSize - 1)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkX + World.ChunkSize, thisChunkZ)));
                }

                if(z == 0)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY, thisChunkZ - World.ChunkSize)));
                }

                if(z == World.ChunkSize - 1)
                {
                    updates.Add(World.BuildChunkName(new Vector3(thisChunkX, thisChunkY, thisChunkZ + World.ChunkSize)));
                }

                foreach(var cName in updates)
                {
                    Chunk chunk;
                    if(World.Chunks.TryGetValue(cName, out chunk))
                    {
                        DestroyImmediate(chunk.ChunkObject.GetComponent<MeshFilter>());
                        DestroyImmediate(chunk.ChunkObject.GetComponent<MeshRenderer>());
                        DestroyImmediate(chunk.ChunkObject.GetComponent<Collider>());
                        chunk.ChunkData[x, y, z].SetType(Block.BlockType.AIR);
                        chunk.DrawChunk();
                    }
                }
            }
        }
    }
}
