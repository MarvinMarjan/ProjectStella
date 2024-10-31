using System;

using SharpNoise;

using SFML.Graphics;


namespace Stella.Game.World;


public readonly record struct NoiseRange(float Start, float End)
{
    public bool IsBetween(float value)
        => value >= Start && value <= End;

    // forces a noise value to be between 0f and 2f. 
    public static float ToValidNoiseValue(float value)
        => Math.Clamp(value, -1f, 1f) + 1f;
}


public static class PerlinNoise
{
    public static void SaveNoiseToFile(NoiseMap noise, string path)
    {
        Image image = new((uint)noise.Width, (uint)noise.Height);

        for (uint x = 0; x < noise.Width; x++)
            for (uint y = 0; y < noise.Height; y++)
            {
                float noiseValue = noise.GetValue((int)x, (int)y);
                byte colorValue = (byte)(NoiseRange.ToValidNoiseValue(noiseValue) * 127.5f);

                image.SetPixel(x, y, new Color(colorValue, colorValue, colorValue));
            }

        image.SaveToFile(path);
    }
}