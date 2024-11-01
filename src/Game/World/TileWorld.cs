using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class TileWorld
{
    public const int DefaultTileSize = 16;
    
    public Tile[,] Tiles { get; }
    public Vector2i Size { get; }

    
    public TileWorld(Vector2i worldSize, int tileSize = DefaultTileSize)
    {
        Tiles = new Tile[worldSize.Y, worldSize.X];
        Size = worldSize;

        InitializeTileArray(worldSize, tileSize);
    }
    
    
    public void DrawAll(GameWindow window)
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