using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GraphNoise : MonoBehaviour 
{

    private float t = 0;
    private float inc = 0.01f;

    private float t2 = 0;
    private float inc2 = 0.001f;

    private float Map(float min, float max, float omin, float omax, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(omin, omax, value));
    }
    
	private void Update() 
	{
        t += inc;
        var n = FractalBrownianMethod(t, 3, 0.8f);
        Grapher.Log(n, "Perlin1", Color.yellow);
	}

    private float FractalBrownianMethod(float t, int octaves, float persistence)
    {
        var total = 0f;
        var frequency = 1f;
        var amplitude = 1f;
        var maxValue = 0f;

        for(var i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(t * frequency, 1f) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2f;
        }

        return total / maxValue;
    }
}
