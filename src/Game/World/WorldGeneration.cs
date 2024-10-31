using SFML.System;

using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class WorldGeneration(int seed)
{
    public int Seed { get; } = seed;
    
    
    public static Perlin DefaultPerlin => new() {
        Frequency = 3f,
        Lacunarity = 1.2f,
        OctaveCount = 8,
        Persistence = 0.8f,
        Quality = NoiseQuality.Best,
    };


    public NoiseMap GenerateNoise(Vector2u worldSize)
    {
        PlaneNoiseMapBuilder builder = new();
        NoiseMap noiseMap = new();
        
        Perlin perlin = DefaultPerlin;
        perlin.Seed = Seed;

        builder.SourceModule = perlin;
        builder.DestNoiseMap = noiseMap;
        builder.SetDestSize((int)worldSize.X, (int)worldSize.Y);
        builder.SetBounds(0, 3, 0, 3);
        builder.Build();

        return noiseMap;
    }


    public TileWorld GenerateWorld(Vector2u worldSize)
    {
        NoiseMap noise = GenerateNoise(worldSize);

        if (GlobalSettings.GeneratedPerlinSavePath is not null)
            PerlinNoise.SaveNoiseToFile(noise, GlobalSettings.GeneratedPerlinSavePath);
        
        return WorldFromNoise(noise);
    }


    public static TileWorld WorldFromNoise(NoiseMap noise)
    {
        TileWorld world = new(new(noise.Width, noise.Height));

        for (int row = 0; row < world.Size.Y; row++)
            for (int col = 0; col < world.Size.X; col++)
            {
                float value = NoiseRange.ToValidNoiseValue(noise.GetValue(col, row));
                world.Tiles[row, col].Object = TileIndex.FromNoiseValue(value);
            }
        
        return world;
    }
}