using SFML.System;
using SFML.Graphics;


namespace Stella.Game;


public static class ViewExtensions
{
    public static FloatRect ViewToRect(this View view)
    {
        Vector2f position = view.Center - view.Size;
        Vector2f size = view.Size * 2;

        return new(position, size);
    }
    

    public static bool IsRectVisibleToView(this View view, FloatRect rect)
        => rect.Intersects(view.ViewToRect());
}