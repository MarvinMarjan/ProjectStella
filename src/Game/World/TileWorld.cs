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
 
    public GameWindow GameWindow { get; private set; }
    public TileSet TileSet { get; set; }
    public Texture TileSetTexture { get; }
    
    // first render to a texture, then render to the window. By doing this,
    // no line glitches occur when zooming the view.
    private readonly RenderTexture _renderingTexture;
    
    private readonly VertexArray _drawingVertices;
    private readonly VertexArray _minimizedDrawingVertices;
    
    public bool MinimizedVerticesUpdateRequested { get; set; }
    
    public float MinimizedDrawingZoomThreshold = 3000f;
    public bool MinimizedDrawing { get; set; }
    
    public Tile[,] Tiles { get; }
    public Vector2u Size { get; }
    
    private readonly Thread _tileUpdateThread;
    private readonly Thread _verticesUpdateThread;
    private readonly CancellationTokenSource _tileUpdateThreadCancellationTokenSource;
    private readonly CancellationTokenSource _verticesUpdateThreadCancellationTokenSource;

    
    public TileWorld(GameWindow window, Vector2u worldSize, int tileSize = DefaultTileSize)
    {
        GameWindow = window;
        
        TileSet = new(8, 8, DefaultTileSize);
        TileSet.LoadFromTileIndex();
        TileSet.SaveToFile(Path.Combine(Environment.CurrentDirectory, "tileset.png"));
        TileSetTexture = new(TileSet);
        TileSetTexture.Smooth = true;
        
        _renderingTexture = new(worldSize.X * DefaultTileSize, worldSize.Y * DefaultTileSize);

        _drawingVertices = new(PrimitiveType.Quads, 4 * worldSize.X * worldSize.Y);
        _minimizedDrawingVertices = new(PrimitiveType.Quads, 4 * worldSize.X * worldSize.Y);
        
        MinimizedDrawing = false;
        
        Tiles = new Tile[worldSize.Y, worldSize.X];
        Size = worldSize;
        
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
            UpdateVertices(!MinimizedVerticesUpdateRequested);
    }


    public void Update(GameWindow window)
    {
        MinimizedDrawing = window.View.Size.X >= MinimizedDrawingZoomThreshold;
    }


    public void Draw(GameWindow window)
    {
        _renderingTexture.Clear();
        
        if (MinimizedDrawing)
            _renderingTexture.Draw(_minimizedDrawingVertices);
        else
            _renderingTexture.Draw(_drawingVertices, new(TileSetTexture));
        
        _renderingTexture.Display();
        
        window.Draw(new Sprite(_renderingTexture.Texture));
    }


    // TODO: use logs to see how the update is happening.
    
    public void UpdateVertices(bool ignoreNonVisible = true)
    {
        uint currentIndex = 0;

        foreach (Tile tile in Tiles)
        {
            if (tile.Object is not null && (tile.Object.IsVisible || !ignoreNonVisible))
            {
                UpdateDrawingVerticesOfTile(tile.Object, currentIndex, MinimizedVerticesUpdateRequested);
            }

            // we are using Quads, so it's 4 vertices
            currentIndex += 4;
        }
        
        MinimizedVerticesUpdateRequested = false;
    }


    private void UpdateDrawingVerticesOfTile(TileDrawable tile, uint currentIndex, bool updateMinimized = false)
    {
        FloatRect bounds = tile.GetLocalBounds();
        
        float x = tile.Position.X;
        float y = tile.Position.Y;
        float sx = bounds.Width;
        float sy = bounds.Height;
        
        if (updateMinimized)
        {
            Color color = TileIndex.LoadedTiles[tile.Name].Color;
            
            _minimizedDrawingVertices[currentIndex + 0] = new(new(x, y), color);
            _minimizedDrawingVertices[currentIndex + 1] = new(new(x + sx, y), color);
            _minimizedDrawingVertices[currentIndex + 2] = new(new(x + sx, y + sy), color);
            _minimizedDrawingVertices[currentIndex + 3] = new(new(x, y + sy), color);
        }
        else
        {
            Vector2i texturePosition = TileSet.GetTilePixelPositionFromIndex(tile.Index);
            
            float tx = texturePosition.X;
            float ty = texturePosition.Y;
            float ts = TileSet.TileSize; // symmetric
            
            _drawingVertices[currentIndex + 0] = new(new(x, y), new Vector2f(tx, ty));
            _drawingVertices[currentIndex + 1] = new(new(x + sx, y), new Vector2f(tx + ts, ty));
            _drawingVertices[currentIndex + 2] = new(new(x + sx, y + sy), new Vector2f(tx + ts, ty + ts));
            _drawingVertices[currentIndex + 3] = new(new(x, y + sy), new Vector2f(tx, ty + ts));
        }
    }


    private void InitializeTileArray(Vector2u worldSize, int tileSize)
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