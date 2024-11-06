using System;
using System.Threading;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using Stella.Areas;
using Stella.Game.Tiles;


namespace Stella.Game.World;


public class TileWorld
{
    public MainGame Game { get; }

    public Tile[,] Tiles { get; }
    public Chunk[,] Chunks { get; }
    public Vector2u Size { get; }
    
    private readonly Thread _chunkUpdateThread;
    private readonly Thread _chunkVerticesUpdateThread;
    private readonly CancellationTokenSource _chunkUpdateThreadCancellationTokenSource;
    private readonly CancellationTokenSource _chunkVerticesUpdateThreadCancellationTokenSource;

    public EventHandler<bool>? MinimizedDrawingChangedEvent;

    
    public TileWorld(MainGame game, Vector2u worldSize)
    {
        if (worldSize.X != worldSize.Y || worldSize.X % Chunk.ChunkSize != 0)
            throw new ArgumentException($"Invalid world size; it must be symmetrical and divisible by {Chunk.ChunkSize}.");

        Game = game;

        Tiles = new Tile[worldSize.Y, worldSize.X];
        Chunks = new Chunk[worldSize.Y / Chunk.ChunkSize, worldSize.X / Chunk.ChunkSize];
        Size = worldSize;

        InitializeTileMatrix();
        InitializeChunks();
        
        _chunkUpdateThreadCancellationTokenSource = new();
        _chunkVerticesUpdateThreadCancellationTokenSource = new();
        _chunkUpdateThread = new (() => ChunkUpdateThread(_chunkUpdateThreadCancellationTokenSource.Token));
        _chunkVerticesUpdateThread = new (() => ChunkVerticesUpdateThread(_chunkUpdateThreadCancellationTokenSource.Token));
        _chunkVerticesUpdateThread.Priority = ThreadPriority.Highest;
    }


    public void StartUpdateThreads()
    {
        _chunkUpdateThread.Start();
        _chunkVerticesUpdateThread.Start();
    }

    public void EndUpdateThreads()
    {
        _chunkUpdateThreadCancellationTokenSource.Cancel();
        _chunkVerticesUpdateThreadCancellationTokenSource.Cancel();
    }


    private void ChunkUpdateThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // TODO: when implementing game logic, using threads for updating stuff will probably suck
            
            Parallel.For(0, Chunks.GetLength(0), new ParallelOptions { MaxDegreeOfParallelism = 3 }, row =>
            {
                for (int col = 0; col < Chunks.GetLength(1); col++)
                    Chunks[row, col].Update();
            });
        }
    }


    private void ChunkVerticesUpdateThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
            foreach (Chunk chunk in Chunks)
                chunk.UpdateVertices();
    }


    public void Update()
    {
        
    }


    public void Draw(RenderTarget window)
    {
        foreach (Chunk chunk in Chunks)
            chunk.Draw(window);
    }
    
    
    private void InitializeTileMatrix()
    {
        const uint tileSize = TileDrawable.DefaultTilePixelSize;
        
        Vector2f position = new(0, 0);

        for (int row = 0; row < Size.Y; row++)
        {
            for (int col = 0; col < Size.X; col++)
            {
                Tiles[row, col] = new(position, tileSize);
                position.X += tileSize;
            }

            position.X = 0;
            position.Y += tileSize;
        }
    }


    private void InitializeChunks()
    {
        for (uint y = 0; y < Chunks.GetLength(0); y++)
            for (uint x = 0; x < Chunks.GetLength(1); x++)
                Chunks[y, x] = new(Game);
                
        for (uint row = 0; row < Tiles.GetLength(0); row++)
            for (uint col = 0; col < Tiles.GetLength(1); col++)
            {
                uint chunkRow = row / Chunk.ChunkSize;
                uint chunkCol = col / Chunk.ChunkSize;
                
                uint relativeTileRow = row % Chunk.ChunkSize;
                uint relativeTileCol = col % Chunk.ChunkSize;
                
                Chunks[chunkRow, chunkCol].Tiles[relativeTileRow, relativeTileCol] = Tiles[row, col];
            }
    }
}