using System.Threading;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class TileWorld
{
    public const int DefaultTileSize = 16;
 
    public GameWindow GameWindow { get; private set; }
    
    public Tile[,] Tiles { get; }
    public Vector2i Size { get; }
    
    private readonly Thread _updateThread;
    private readonly CancellationTokenSource _updateThreadCancellationTokenSource;

    private VertexArray MinimizedDrawingVertices { get; }
    public float MinimizedDrawingZoomThreshold = 3000f;
    public bool UpdateMinimizedVerticesRequested { get; set; }
    
    public bool MinimizedDrawing { get; set; }

    
    public TileWorld(GameWindow window, Vector2i worldSize, int tileSize = DefaultTileSize)
    {
        GameWindow = window;
        
        Tiles = new Tile[worldSize.Y, worldSize.X];
        Size = worldSize;

        InitializeTileArray(worldSize, tileSize);

        _updateThreadCancellationTokenSource = new();
        _updateThread = new Thread(() => UpdateThread(_updateThreadCancellationTokenSource.Token));

        MinimizedDrawingVertices = new(PrimitiveType.Quads, (uint)(4 * Size.X * Size.Y));
        MinimizedDrawing = false;
        UpdateMinimizedVerticesRequested = true;
    }


    public void StartUpdateThread()
        => _updateThread.Start();

    public void EndUpdateThread()
        => _updateThreadCancellationTokenSource.Cancel();


    private void UpdateThread(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
            Parallel.For(0, Tiles.GetLength(0), new ParallelOptions { MaxDegreeOfParallelism = 3 }, row =>
            {
                for (int col = 0; col < Tiles.GetLength(1); col++)
                    Tiles[row, col].Object?.Update(GameWindow);
            });
    }


    public void Update(GameWindow window)
    {
        MinimizedDrawing = window.View.Size.X >= MinimizedDrawingZoomThreshold;
    }
    
    
    public void Draw(GameWindow window)
    {
        if (MinimizedDrawing)
        {
            MinimizedDraw(window);
            return;
        }

        foreach (Tile tile in Tiles)
            tile.Object?.Draw(window);
    }


    public void MinimizedDraw(GameWindow window)
    {
        if (UpdateMinimizedVerticesRequested)
        {
            UpdateMinimizedVertices();
            UpdateMinimizedVerticesRequested = false;
        }

        window.Draw(MinimizedDrawingVertices);
    }


    public void UpdateMinimizedVertices()
    {
        uint currentIndex = 0;

        foreach (Tile tile in Tiles)
        {
            if (tile.Object is not null)
                AddMinimizedDrawingVertices(tile.Object, currentIndex);

            currentIndex += 4;
        }
    }


    private void AddMinimizedDrawingVertices(TileDrawable tile, uint currentIndex)
    {
        FloatRect bounds = tile.GetLocalBounds();
                
        float x = tile.Position.X;
        float y = tile.Position.Y;
        float sx = bounds.Width;
        float sy = bounds.Height;

        Color color = TileIndex.TileColor[tile.Name];

        MinimizedDrawingVertices[currentIndex + 0] = new(new(x, y), color);
        MinimizedDrawingVertices[currentIndex + 1] = new(new(x + sx, y), color);
        MinimizedDrawingVertices[currentIndex + 2] = new(new(x + sx, y + sy), color);
        MinimizedDrawingVertices[currentIndex + 3] = new(new(x, y + sy), color);
    }


    private void InitializeTileArray(Vector2i worldSize, int tileSize)
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