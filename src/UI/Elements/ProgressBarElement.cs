using System;

using SFML.System;
using SFML.Graphics;

using Stella.UI.Elements.Shapes;


namespace Stella.UI.Elements;


public class ProgressBarElement : Element
{
    public override Transformable Transformable => Foreground.Transformable;

    public RectangleElement Foreground { get; }
    public RectangleElement Background { get; }

    private float _progress;
    public float Progress
    {
        get => _progress;
        set
        {
            if (value >= MinValue && value <= MaxValue)
                _progress = value;
            else
                throw new ArgumentOutOfRangeException(nameof(value), "Progress must be between MinValue and MaxValue.");
        }
    }

    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    
    public bool IsAtMax => Progress >= MaxValue;
    public bool IsAtMin => Progress <= MinValue;


    public ProgressBarElement(Element? parent, Vector2f position, Vector2f size, float minValue = 0f, float maxValue = 1f) : base(parent)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        
        Progress = minValue;
        
        Position = position;

        Background = new(this, new(), size)
        {
            Color = Color.Black
        };
        
        Foreground = new(this, new(), size)
        {
            Color = Color.White
        };
    }


    public override void Update()
    {
        if (!Visible)
            return;
        
        float normalizedProgress = (Progress - MinValue) / (MaxValue - MinValue);
        Foreground.Size = new(Background.Size.X * normalizedProgress, Background.Size.Y);

        base.Update();
    }


    public override FloatRect GetBounds()
        => Background.GetBounds(); // only foreground change its size; use background then
}