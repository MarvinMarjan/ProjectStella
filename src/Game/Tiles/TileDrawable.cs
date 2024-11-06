using System;

using SFML.System;
using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileDrawable : Sprite, IDrawable, IUpdateable, ICloneable
{
    public const int DefaultTilePixelSize = 16;
    
    public string Name { get; }
    public int Index { get; }

    public TileTextureAnimation TileTextureAnimation { get; }
    

    public TileDrawable(string name) : this(name, TileIndex.LoadedTiles[name].TextureAnimation)
    { }


    private TileDrawable(string name, TileTextureAnimation source) : base(source[0])
    {
        Name = name;
        Index = TileIndex.GetTileIndexByName(Name);
        TileTextureAnimation = source;
    }


    public void Update()
    {
        
    }


    public void Draw(RenderTarget target)
    {
        target.Draw(this);
    }


    public void SetScaleToPixels(Vector2f size)
    {
        FloatRect localBounds = GetGlobalBounds();
        Scale = new(size.X / localBounds.Width, size.Y / localBounds.Height);
    }


    public object Clone()
        => new TileDrawable(Name, TileTextureAnimation);
}