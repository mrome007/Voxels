using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    private static int maxHeight = 150;
    private static float smooth = 0.01f;
    static int octaves = 4;
    static float persistence = 0.5f;

    public static int GenerateHeight(float x, float z)
    {
        var height = Map(0, maxHeight, 0, 1, FractalBrownianMethod(x * smooth, z * smooth, octaves, persistence));
        return (int)height;
    }

    public static int GenerateStoneHeight(float x, float z)
    {
        var height = Map(0, maxHeight - 5, 0, 1, FractalBrownianMethod(x * smooth * 2f, z * smooth * 2f, octaves + 1, persistence));
        return (int)height;
    }

    private static float Map(float newMin, float newMax, float omin, float omax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(omin, omax, value));
    }

    private static float FractalBrownianMethod(float x, float z, int octaves, float persistence)
    {
        var total = 0f;
        var frequency = 1f;
        var amplitude = 1f;
        var maxValue = 0f;

        var offset = 32000f;

        for(var i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2f;
        }

        return total / maxValue;
    }

    public static float FractalBrownianMethod3D(float x, float y, float z, float sm, int oct)
    {
        var xy = FractalBrownianMethod(x * sm, y * sm, oct, 0.5f);
        var yz = FractalBrownianMethod(y * sm, z * sm, oct, 0.5f);
        var xz = FractalBrownianMethod(x * sm, z * sm, oct, 0.5f);

        var yx = FractalBrownianMethod(y * sm, x * sm, oct, 0.5f);
        var zy = FractalBrownianMethod(z * sm, y * sm, oct, 0.5f);
        var zx = FractalBrownianMethod(z * sm, x * sm, oct, 0.5f);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
}
