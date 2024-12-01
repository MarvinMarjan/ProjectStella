using System;
using System.Threading;
using System.Threading.Tasks;

using SFML.System;
using SFML.Graphics;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public enum WorldGenerationStage
{
    None,
    
    NoiseGeneration,
    WorldInitialization,
    WorldTerrainFilling,
    LoadingChunks,
    Finished
}


public class WorldNoiseGenerator(int? seed = null)
{
    public int Seed { get; } = seed ?? new Random().Next();
    
    
    public FastNoiseLite GetDefaultNoise()
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

    public FastNoiseLite GetTerrainHeightMultiplierNoise()
    {
        FastNoiseLite noise = GetDefaultNoise();
        noise.SetFrequency(0.0007f);
        noise.SetFractalOctaves(1);

        return noise;
    }
    
    
    public static float[,] GenerateNoiseMatrix(Vector2u worldSize, FastNoiseLite noise)
        => noise.FastNoiseLiteToFloatMatrix(worldSize.X, worldSize.Y);


    public static float[,] GenerateWorldNoise(Vector2u worldSize, int? seed = null)
    {
        WorldNoiseGenerator generator = new(seed);

        float[,] baseNoise = GenerateNoiseMatrix(worldSize, generator.GetDefaultNoise());
        float[,] heightFactorNoise = GenerateNoiseMatrix(worldSize, generator.GetTerrainHeightMultiplierNoise());
        float[,] finalNoise = new float[worldSize.Y, worldSize.X];

        for (int row = 0; row < worldSize.Y; row++)
            for (int col = 0; col < worldSize.X; col++)
                finalNoise[row, col] = baseNoise[row, col] + heightFactorNoise[row, col];

        return finalNoise;
    }
}


public class WorldGenerator
{
    public TileWorld? TileWorld { get; private set; }
    public WorldGenerationStage Stage { get; private set; }

    public View View { get; }
    public Vector2u WorldSize { get; }
    public int Seed { get; }

    private Thread? _worldGenerationThread;

    
    public WorldGenerator(View view, Vector2u worldSize, int? seed = null)
    {
        Stage = WorldGenerationStage.None;
        
        View = view;
        WorldSize = worldSize;
        Seed = seed ?? new Random().Next();
    }


    public static TileWorld GenerateWorld(View view, Vector2u worldSize, int? seed = null)
    {
        WorldGenerator generator = new(view, worldSize, seed);
        generator.StartWorldGeneration();
        generator.WaitWorldGenerationFinish();

        return generator.TileWorld!;
    }
    

    public void StartWorldGeneration()
    {
        if (_worldGenerationThread is not null && _worldGenerationThread.IsAlive)
            throw new InvalidOperationException("World generation has already been started.");
            
        _worldGenerationThread = new(WorldGenerationThread);
        _worldGenerationThread.Priority = ThreadPriority.Highest;
        _worldGenerationThread.Start();
    }
    
    public void WaitWorldGenerationFinish()
        => _worldGenerationThread?.Join();


    private void WorldGenerationThread()
    {
        Stage = WorldGenerationStage.NoiseGeneration;
        float[,] noise = WorldNoiseGenerator.GenerateWorldNoise(WorldSize, Seed);

        Stage++; TileWorld = new(View, WorldSize);
        Stage++; FillWorldFromNoise(TileWorld, noise);
        Stage++; TileWorld.UpdateAllChunksVertices(true);
        Stage++;

        View.Center = TileWorld.GetCenterPosition();
    }


    public static void FillWorldFromNoise(TileWorld world, float[,] noise)
        => Parallel.For(0, world.TileCount.Y, new() { MaxDegreeOfParallelism = 5}, row => {
                for (uint col = 0; col < world.TileCount.X; col++)
                {
                    float value = NoiseRange.ToUnsignedNoiseValue(noise[row, col]);
                    world.Tiles[row, col].Object = TileIndex.FromNoiseValue(value);
                }
        });
    
    
    public static float CalculateWorldGenerationProgress(TileWorld world)
    {
        uint worldSize = world.TileCount.X * world.TileCount.Y;
        uint counter = 0;
        
        foreach (Tile tile in world.Tiles)
            if (tile.Object is not null)
                counter++;
        
        return counter / (float)worldSize;
    }


    public static string WorldGenerationStageToString(WorldGenerationStage stage) => stage switch
    {
        WorldGenerationStage.None => "Starting",
        WorldGenerationStage.NoiseGeneration => "Generating noise",
        WorldGenerationStage.WorldInitialization => "Initializing world",
        WorldGenerationStage.WorldTerrainFilling => "Filling terrain",
        WorldGenerationStage.LoadingChunks => "Loading chunks",
        
        _ => "Doing something"
    };
}