using SFML.Graphics;
using SFML.System;

using Stella.Game.Tiles;


namespace Stella.Game.World;


public class Chunk
{
    public const uint ChunkSize = 64;
    
    public GameWindow GameWindow { get;  }
    
    public Tile[,] Tiles { get; }
    public TileMapRenderer Renderer { get; }

    public Vector2f Position => Tiles[0, 0].Position;
    public Vector2f Size => new(ChunkSize * TileDrawable.DefaultTilePixelSize, ChunkSize * TileDrawable.DefaultTilePixelSize);


    public Chunk(GameWindow gameWindow)
    {
        GameWindow = gameWindow;
        
        Tiles = new Tile[ChunkSize, ChunkSize];
        Renderer = new(Tiles);
        Renderer.Tiles = Tiles;
    }


    public void Update()
    {
        for (int row = 0; row < ChunkSize; row++)
            for (int col = 0; col < ChunkSize; col++)
                Tiles[row, col].Object?.Update(GameWindow);
    }


    public void UpdateVertices()
    {
        if (!IsVisibleToWindow())
            return;
        
        Renderer.UpdateVertices();
    }


    public void Draw(RenderTexture renderTexture, bool renderOutline = false)
    {
        if (!IsVisibleToWindow())
            return;
        
        Renderer.AddRender(renderTexture);
        
        if (renderOutline)
            renderTexture.Draw(new RectangleShape(Size)
            {
                Position = Position,
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 2f * (GameWindow.View.Size.X / GameWindow.Camera.DefaultViewSize.X)
            });
    }


    public bool IsVisibleToWindow()
        => GameWindow.Camera.IsRectVisibleToCamera(new(Position, Size));
}