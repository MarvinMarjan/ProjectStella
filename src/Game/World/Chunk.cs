using SFML.Graphics;
using SFML.System;

using Stella.Areas;
using Stella.Game.Tiles;


namespace Stella.Game.World;


public class Chunk : IDrawable, IUpdateable
{
    public const uint ChunkSize = 64;
    
    public MainGame Game { get;  }
    
    public Tile[,] Tiles { get; }
    public TileMapRenderer Renderer { get; }

    public Vector2f Position => Tiles[0, 0].Position;
    public Vector2f Size => new(ChunkSize * TileDrawable.DefaultTilePixelSize, ChunkSize * TileDrawable.DefaultTilePixelSize);


    public Chunk(MainGame game)
    {
        Game = game;
        
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


    public void Draw(RenderTarget target, bool renderOutline)
    {
        if (!IsVisibleToWindow())
            return;
        
        Renderer.Render(target);
        
        if (renderOutline)
            target.Draw(new RectangleShape(Size)
            {
                Position = Position,
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 2f * (Game.View.Size.X / Game.Camera.DefaultViewSize.X)
            });
    }

    public void Draw(RenderTarget target)
        => Draw(target, false);


    public bool IsVisibleToWindow()
        => Game.Camera.IsRectVisibleToCamera(new(Position, Size));
}