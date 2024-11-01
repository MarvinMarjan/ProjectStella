using System;

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


public static class PerlinNoiseUtils
{
    public static float[,] FastNoiseLiteToFloatMatrix(this FastNoiseLite noise, uint width, uint height)
    {
        float[,] noiseValues = new float[height, width];
        
        for (int row = 0; row < height; row++)
            for (int col = 0; col < width; col++)
                noiseValues[row, col] = noise.GetNoise(col, row);

        return noiseValues;
    }
    
    
    public static void SaveNoiseToFile(float[,] noise, string path)
    {
        uint width = (uint)noise.GetLength(1);
        uint height = (uint)noise.GetLength(0);
        
        Image image = new(width, height);

        for (uint x = 0; x < width; x++)
            for (uint y = 0; y < height; y++)
            {
                byte colorValue = (byte)(NoiseRange.ToValidNoiseValue(noise[y, x]) * 127.5f);
                image.SetPixel(x, y, new Color(colorValue, colorValue, colorValue));
            }

        image.SaveToFile(path);
    }
}