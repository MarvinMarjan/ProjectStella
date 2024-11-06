using SFML.System;
using SFML.Graphics;


namespace Stella.UI.Elements.Shapes;


public abstract class ShapeElement(Element? parent, Shape shape) : Element(parent)
{
    public Shape SfmlShape { get; } = shape;

    public Vector2f Position
    {
        get => SfmlShape.Position;
        set => SfmlShape.Position = value;
    }

    public float Rotation
    {
        get => SfmlShape.Rotation;
        set => SfmlShape.Rotation = value;
    }

    public float OutlineThickness
    {
        get => SfmlShape.OutlineThickness;
        set => SfmlShape.OutlineThickness = value;
    }

    public Color FillColor
    {
        get => SfmlShape.FillColor;
        set => SfmlShape.FillColor = value;
    }
    
    public Color OutlineColor
    {
        get => SfmlShape.OutlineColor;
        set => SfmlShape.OutlineColor = value;
    }
    

    public override void Draw(RenderTarget target)
    {
        target.Draw(SfmlShape);
        
        base.Draw(target);
    }
}