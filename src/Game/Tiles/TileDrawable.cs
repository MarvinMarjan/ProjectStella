using System;

using SFML.System;
using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileDrawable(Texture source) : Sprite(source), ICloneable
{
    public bool IsVisible { get; private set; }


    public TileDrawable(string sourcePath) : this(new Texture(sourcePath))
    {}
    
    public TileDrawable() : this(new Texture(1, 1))
    {}


    public void Update(GameWindow window)
    {
        View windowView = window.CurrentView;

        Vector2f windowPosition = windowView.Center - windowView.Size / 2;
        FloatRect windowViewRect = new(windowPosition, windowView.Size);

        IsVisible = GetGlobalBounds().Intersects(windowViewRect);
    }


    public void Draw(GameWindow window)
    {
        if (IsVisible)
            window.Draw(this);
    }


    public void SetScaleToPixels(Vector2f size)
    {
        FloatRect localBounds = GetLocalBounds();
        Scale = new(size.X / localBounds.Width, size.Y / localBounds.Height);
    }


    public object Clone()
        => new TileDrawable(Texture);
}