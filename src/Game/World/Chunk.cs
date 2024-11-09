using SFML.System;
using SFML.Graphics;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class Chunk : IDrawable, IUpdateable
{
    public const uint ChunkSize = 64;
    
    public View View { get; }
    
    public Tile[,] Tiles { get; }
    public TileMapRenderer Renderer { get; }

    public Vector2f Position => Tiles[0, 0].Position;
    public Vector2f Size => new(ChunkSize * TileDrawable.DefaultTilePixelSize, ChunkSize * TileDrawable.DefaultTilePixelSize);


    public Chunk(View view)
    {
        View = view;
        
        Tiles = new Tile[ChunkSize, ChunkSize];
        Renderer = new(Tiles);
        Renderer.Tiles = Tiles;
    }


    public void Update()
    {
        for (int row = 0; row < ChunkSize; row++)
            for (int col = 0; col < ChunkSize; col++)
                Tiles[row, col].Object?.Update();
    }


    public void UpdateVertices()
    {
        if (!IsVisibleToWindow())
            return;
        
        Renderer.UpdateVertices();
    }


    public void Draw(RenderTarget target)
    {
        if (!IsVisibleToWindow())
            return;
        
        Renderer.Render(target);
    }


    public bool IsVisibleToWindow()
        => View.IsRectVisibleToView(new(Position, Size));
}