using System;

using SFML.System;
using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileDrawable : Sprite, ICloneable
{
    public string Name { get; }
    public bool IsVisible { get; private set; }

    public TileTextureAnimation TileTextureAnimation { get; }
    

    // TODO: cloning the texture may be important; test if it's behaving as a reference
    public TileDrawable(string name) : this(name, new(TileIndex.LoadedTiles[name]))
    { }


    private TileDrawable(string name, TileTextureAnimation source) : base(source[0])
    {
        Name = name;
        TileTextureAnimation = source;
    }


    public void Update(GameWindow window)
    {
        IsVisible = window.Camera.IsRectVisibleToCamera(GetGlobalBounds());
    }


    public void Draw(GameWindow window)
    {
        if (IsVisible)
            window.Draw(this);
    }


    public void SetScaleToPixels(Vector2f size)
    {
        FloatRect localBounds = GetGlobalBounds();
        Scale = new(size.X / localBounds.Width, size.Y / localBounds.Height);
    }


    public object Clone()
        => new TileDrawable(Name, TileTextureAnimation);
}