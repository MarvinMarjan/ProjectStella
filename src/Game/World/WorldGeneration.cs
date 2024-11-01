using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class WorldGeneration(int seed)
{
    public int Seed { get; } = seed;


    public FastNoiseLite GetDefaultNoise()
    {
        FastNoiseLite noise = new(Seed);
        
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.005f);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(2f);
        noise.SetFractalGain(0.5f);
        noise.SetFractalWeightedStrength(0f);
        noise.SetFractalPingPongStrength(2f);
        noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
        noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance);
        noise.SetCellularJitter(1f);
        // noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2Reduced);
        // noise.SetDomainWarpAmp(3000f);
        
        return noise;
    }


    public float[,] GenerateNoise(Vector2u worldSize)
        => GetDefaultNoise().FastNoiseLiteToFloatMatrix(worldSize.X, worldSize.Y);


    public TileWorld GenerateWorld(Vector2u worldSize)
    {
        float[,] noise = GenerateNoise(worldSize);

        if (GlobalSettings.GeneratedPerlinSavePath is not null)
            PerlinNoiseUtils.SaveNoiseToFile(noise, GlobalSettings.GeneratedPerlinSavePath);
        
        return WorldFromNoise(noise);
    }


    public static TileWorld WorldFromNoise(float[,] noise)
    {
        TileWorld world = new(new(noise.GetLength(1), noise.GetLength(0)));

        for (int row = 0; row < world.Size.Y; row++)
            for (int col = 0; col < world.Size.X; col++)
            {
                float value = NoiseRange.ToValidNoiseValue(noise[row, col]);
                world.Tiles[row, col].Object = TileIndex.FromNoiseValue(value);
            }
        
        return world;
    }
}