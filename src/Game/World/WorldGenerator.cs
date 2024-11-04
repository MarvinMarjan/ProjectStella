using System;
using System.Threading.Tasks;

using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class WorldGenerator(int seed)
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


    public static async Task FillWorldFromNoiseAsync(TileWorld world, float[,] noise)
        => await Task.Run(() =>
        {
            Parallel.For(0, world.Size.Y, new() { MaxDegreeOfParallelism = 5}, row => {
                for (uint col = 0; col < world.Size.X; col++)
                {
                    float value = NoiseRange.ToValidNoiseValue(noise[row, col]);
                    world.Tiles[row, col].Object = TileIndex.FromNoiseValue(value);
                }
            });
        });


    public static TileWorld GenerateWorld(GameWindow window, Vector2u worldSize, int? seed)
    {
        float[,] noise = new WorldGenerator(seed ?? new Random().Next()).GenerateNoise(worldSize);

        TileWorld world = new(window, worldSize);
        FillWorldFromNoiseAsync(world, noise).Wait();

        return world;
    }
}