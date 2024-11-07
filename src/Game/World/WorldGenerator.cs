using System;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class WorldGenerator(int seed)
{
    public int Seed { get; } = seed;


    private FastNoiseLite GetDefaultNoise()
    {
        FastNoiseLite noise = new(Seed);
        
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.0035f);
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

    private FastNoiseLite GetTerrainHeightMultiplierNoise()
    {
        FastNoiseLite noise = GetDefaultNoise();
        noise.SetFrequency(0.0007f);
        noise.SetFractalOctaves(1);

        return noise;
    }

    
    private float[,] GenerateNoise(Vector2u worldSize, FastNoiseLite noise)
        => noise.FastNoiseLiteToFloatMatrix(worldSize.X, worldSize.Y);


    public static async Task FillWorldFromNoiseAsync(TileWorld world, float[,] noise)
        => await Task.Run(() =>
        {
            Parallel.For(0, world.Size.Y, new() { MaxDegreeOfParallelism = 5}, row => {
                for (uint col = 0; col < world.Size.X; col++)
                {
                    float value = NoiseRange.ToUnsignedNoiseValue(noise[row, col]);
                    world.Tiles[row, col].Object = TileIndex.FromNoiseValue(value);
                }
            });
        });


    public static float[,] GenerateWorldNoise(Vector2u worldSize, int? seed)
    {
        WorldGenerator generator = new(seed ?? new Random().Next());
        
        float[,] baseNoise = generator.GenerateNoise(worldSize, generator.GetDefaultNoise());
        float[,] heightFactorNoise = generator.GenerateNoise(worldSize, generator.GetTerrainHeightMultiplierNoise());
        float[,] finalNoise = new float[worldSize.Y, worldSize.X];
        
        for (int row = 0; row < worldSize.Y; row++)
            for (int col = 0; col < worldSize.X; col++)
                finalNoise[row, col] = baseNoise[row, col] + heightFactorNoise[row, col];

        return finalNoise;
    }


    public static TileWorld GenerateWorld(View view, Vector2u worldSize, int? seed)
    {
        float[,] noise = GenerateWorldNoise(worldSize, seed);

        TileWorld world = new(view, worldSize);
        FillWorldFromNoiseAsync(world, noise).Wait();

        return world;
    }
}