using Latte.Core.Type;
using Latte.Elements.Primitives;


namespace Stella.GUI;


public class Button : ButtonElement
{
    public Button(Element? parent, Vec2f position, Vec2f size, string text) : base(parent, position, size, text)
    {
        Radius.Set(5f);
        
        Color.Set(new(200, 200, 200));
        BorderColor.Set(SFML.Graphics.Color.Transparent);
        BorderSize.Set(0f);
        
        Text.Size.Set(20);
        
        Animator.Time = 0.1f;
        
        Hover = new()
        {
            { "Color", new ColorRGBA(160, 160, 160) }
        };
        
        Down = new()
        {
            { "Color", new ColorRGBA(120, 120, 120) }
        };
        
        UseDefaultAnimation = false;
    }
}