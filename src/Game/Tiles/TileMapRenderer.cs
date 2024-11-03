using System;

using SFML.Graphics;
using SFML.System;


namespace Stella.Game.Tiles;


public class TileMapRenderer
{
    public Tile[,] Tiles { get; }
    public TileSet TileSet { get; }
    private readonly Texture _tileSetTexture;
    
    public VertexArray TileVertices { get; }
    public bool _init;
    public bool NoTextures { get; set; }

    public EventHandler? FirstVerticesUpdatedEvent;
    

    public TileMapRenderer(Tile[,] tiles, TileSet tileSet, uint tileSize)
    {
        Tiles = tiles;
        TileSet = tileSet;
        _tileSetTexture = new(TileSet);
        _tileSetTexture.Smooth = true;
        
        TileVertices = new(PrimitiveType.Quads, (uint)(4 * tiles.GetLength(1) * tiles.GetLength(0)));
        _init = true;
    }


    public void Render(RenderTexture texture)
    {
        texture.Clear();
        
        if (NoTextures)
            texture.Draw(TileVertices);
        else
            texture.Draw(TileVertices, new(_tileSetTexture));
        
        texture.Display();
    }


    public void UpdateVertices()
    {
        uint currentIndex = 0;

        foreach (Tile tile in Tiles)
        {
            if (tile.Object is not null)
                UpdateVerticesOfTile(tile.Object, currentIndex);
                                
            // we are using Quads, so it's 4 vertices
            currentIndex += 4;
        }

        if (_init)
        {
            FirstVerticesUpdatedEvent?.Invoke(this, EventArgs.Empty);
            _init = false;
        }
    }


    private void UpdateVerticesOfTile(TileDrawable tile, uint currentIndex)
    {
        FloatRect bounds = tile.GetLocalBounds();
        
        float x = tile.Position.X;
        float y = tile.Position.Y;
        float sx = bounds.Width;
        float sy = bounds.Height;
        
        if (NoTextures)
        {
            Color color = TileIndex.LoadedTiles[tile.Name].Color;
            
            TileVertices[currentIndex + 0] = new(new(x, y), color);
            TileVertices[currentIndex + 1] = new(new(x + sx, y), color);
            TileVertices[currentIndex + 2] = new(new(x + sx, y + sy), color);
            TileVertices[currentIndex + 3] = new(new(x, y + sy), color);
        }
        else
        {
            Vector2i texturePosition = TileSet.GetTilePixelPositionFromIndex(tile.Index);
        
            float tx = texturePosition.X;
            float ty = texturePosition.Y;
            float ts = TileSet.TileSize; // symmetric
        
            TileVertices[currentIndex + 0] = new(new(x, y), new Vector2f(tx, ty));
            TileVertices[currentIndex + 1] = new(new(x + sx, y), new Vector2f(tx + ts, ty));
            TileVertices[currentIndex + 2] = new(new(x + sx, y + sy), new Vector2f(tx + ts, ty + ts));
            TileVertices[currentIndex + 3] = new(new(x, y + sy), new Vector2f(tx, ty + ts));
        }
    }
}