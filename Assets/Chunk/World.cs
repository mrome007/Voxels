using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject Player;
    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static int WorldSize = 2;
    public static int Radius = 1;
    public static Dictionary<string, Chunk> Chunks;

    public Slider loadingAmount;
    public Button playButton;
    public Camera uiCamera;

    public static string BuildChunkName(Vector3 position)
    {
        return (int)position.x + "_" + (int)position.y + "_" + (int)position.z;
    }

    private IEnumerator BuildChunkColumn()
    {
        for(var index = 0; index < ColumnHeight; index++)
        {
            var chunkPosition = new Vector3(this.transform.position.x, index * ChunkSize, this.transform.position.z);
            var chunk = new Chunk(chunkPosition, TextureAtlas);
            chunk.ChunkObject.transform.parent = this.transform;
            Chunks.Add(chunk.ChunkObject.name, chunk);
        }

        foreach(var chunk in Chunks)
        {
            chunk.Value.DrawChunk();
            yield return null;
        }
    }

    private IEnumerator BuildWorld()
    {
        var posX = (int)Mathf.Floor(Player.transform.position.x / ChunkSize);
        var posZ = (int)Mathf.Floor(Player.transform.position.z / ChunkSize);

        var totalChunks = (Mathf.Pow(Radius * 2 + 1, 2) * ColumnHeight) * 2;
        var processCount = 0;

        for(int z = -Radius; z <= Radius; z++)
        {
            for(int x = -Radius; x <= Radius; x++)
            {
                for(int y = 0; y < ColumnHeight; y++)
                {
                    var chunkPosition = new Vector3((x + posX) * ChunkSize, y * ChunkSize, (z + posZ) * ChunkSize);
                    var chunk = new Chunk(chunkPosition, TextureAtlas);
                    chunk.ChunkObject.transform.parent = this.transform;
                    Chunks.Add(chunk.ChunkObject.name, chunk);

                    processCount++;
                    loadingAmount.value = processCount / totalChunks * 100;
                    yield return null;
                }
            }
        }

        foreach(var chunk in Chunks)
        {
            chunk.Value.DrawChunk();
            processCount++;
            loadingAmount.value = processCount / totalChunks * 100;
            yield return null;
        }

        Player.SetActive(true);

        ShowWorldUI(false);
    }

    public void StartBuild()
    {
        playButton.interactable = false;
        StartCoroutine(BuildWorld());
    }

    public void ShowWorldUI(bool show)
    {
        playButton.interactable = show;
        loadingAmount.gameObject.SetActive(show);
        playButton.gameObject.SetActive(show);
        uiCamera.gameObject.SetActive(show);
    }

    private void Start()
    {
        Player.SetActive(false);
        Chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        ShowWorldUI(true);
    }
}
