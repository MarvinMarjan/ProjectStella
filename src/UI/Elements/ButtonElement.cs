using SFML.System;

using Stella.UI.Elements.Shapes;


namespace Stella.UI.Elements;


public class ButtonElement : RectangleElement
{
    public TextElement Text { get; set; }
    
    
    public ButtonElement(Element? parent, Vector2f position, Vector2f size, string text) : base(parent, position, size)
    {
        Text = new(this, position, 20, text);
    }
}