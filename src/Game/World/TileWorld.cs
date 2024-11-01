using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    
    public TileWorld(GameWindow window, Vector2i worldSize, int tileSize = DefaultTileSize)
    {
        GameWindow = window;
        
        Tiles = new Tile[worldSize.Y, worldSize.X];
        Size = worldSize;

        InitializeTileArray(worldSize, tileSize);

        _updateThreadCancellationTokenSource = new();
        _updateThread = new Thread(() => UpdateThread(_updateThreadCancellationTokenSource.Token));
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
                    Tiles[row, col].Update(GameWindow);
            });
    }
    
    
    public void Draw(GameWindow window)
    {
        foreach (Tile tile in Tiles)
            tile.Draw(window);
    }
    

    public void FillAllWith(TileDrawable @object)
    {
        // we don't want a single object inside all of those tiles,
        // so it's important to clone it.
        foreach (Tile tile in Tiles)
            tile.Object = @object.Clone() as TileDrawable;
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