using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class TileWorld
{
    public const int DefaultTileSize = 16;
 
    public GameWindow GameWindow { get; }
    public TileMapRenderer Renderer { get; }
    public RenderTexture RenderTexture { get; }
    public RenderTexture MinimizedRenderTexture { get; }
    public bool UpdateMinimizedRenderTextureRequested { get; set; }
    
    public float MinimizedDrawingZoomThreshold { get; set; }
    public bool MinimizedDrawing { get; set; }
    
    public Tile[,] Tiles { get; }
    public Vector2u Size { get; }
    
    private readonly Thread _tileUpdateThread;
    private readonly Thread _verticesUpdateThread;
    private readonly CancellationTokenSource _tileUpdateThreadCancellationTokenSource;
    private readonly CancellationTokenSource _verticesUpdateThreadCancellationTokenSource;

    
    public TileWorld(GameWindow window, Vector2u worldSize, uint tileSize = DefaultTileSize)
    {
        GameWindow = window;
        
        TileSet tileSet = new(8, 8, DefaultTileSize);
        tileSet.LoadFromTileIndex();
        tileSet.SaveToFile(Path.Combine(Environment.CurrentDirectory, "tileset.png"));
        
        Tiles = new Tile[worldSize.Y, worldSize.X];
        Size = worldSize;
        
        Renderer = new(Tiles, tileSet, tileSize);
        RenderTexture = new(worldSize.X * tileSize, worldSize.Y * tileSize);
        MinimizedRenderTexture = new(worldSize.X * tileSize, worldSize.Y * tileSize);

        Renderer.FirstVerticesUpdatedEvent += (_, _) => UpdateMinimizedRenderTextureRequested = true;

        MinimizedDrawingZoomThreshold = 4000f;
        
        _tileUpdateThreadCancellationTokenSource = new();
        _verticesUpdateThreadCancellationTokenSource = new();
        _tileUpdateThread = new (() => TileUpdateThread(_tileUpdateThreadCancellationTokenSource.Token));
        _verticesUpdateThread = new(() => VerticesUpdateThread(_verticesUpdateThreadCancellationTokenSource.Token));
        
        InitializeTileArray(worldSize, tileSize);
    }


    public void StartUpdateThreads()
    {
        _tileUpdateThread.Start();
        _verticesUpdateThread.Start();
    }

    public void EndUpdateThreads()
    {
        _tileUpdateThreadCancellationTokenSource.Cancel();
        _verticesUpdateThreadCancellationTokenSource.Cancel();
    }


    private void TileUpdateThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Parallel.For(0, Tiles.GetLength(0), new ParallelOptions { MaxDegreeOfParallelism = 3 }, row =>
            {
                for (int col = 0; col < Tiles.GetLength(1); col++)
                    Tiles[row, col].Object?.Update(GameWindow);
            });
        }
    }

    private void VerticesUpdateThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
            Renderer.UpdateVertices();
    }


    public void Update(GameWindow window)
    {
        MinimizedDrawing = window.View.Size.X >= MinimizedDrawingZoomThreshold;
    }


    public void Draw(GameWindow window)
    {
        if (UpdateMinimizedRenderTextureRequested)
        {
            UpdateMinimizedRenderTexture();
            UpdateMinimizedRenderTextureRequested = false;
        }
        
        if (!MinimizedDrawing)
            Renderer.Render(RenderTexture);
        
        window.Draw(new Sprite(MinimizedDrawing ? MinimizedRenderTexture.Texture : RenderTexture.Texture));
    }


    public void UpdateMinimizedRenderTexture()
    {
        Renderer.NoTextures = true;
        Renderer.UpdateVertices();
        Renderer.Render(MinimizedRenderTexture);
        Renderer.NoTextures = false;
    }


    private void InitializeTileArray(Vector2u worldSize, uint tileSize)
    {
        Vector2f position = new(0, 0);

        for (int row = 0; row < worldSize.Y; row++)
        {
            for (int col = 0; col < worldSize.X; col++)
            {
                Tiles[row, col] = new(position, tileSize);
                
                position.X += tileSize;
            }

            position.X = 0;
            position.Y += tileSize;
        }
    }
}