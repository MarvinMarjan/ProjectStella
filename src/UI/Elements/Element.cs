using System.Collections.Generic;

using SFML.System;
using SFML.Graphics;


namespace Stella.UI.Elements;


public abstract class Element : IUpdateable, IDrawable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    public abstract Transformable Transformable { get; }
    
    public Vector2f Position { get; set; }
    public Vector2f AbsolutePosition
    {
        get => Parent is not null ? Position + Parent.Position : Position;
        set
        {
            if (Parent is not null)
                Position = value - Parent.Position;
            else
                Position = value;
        }
    }

    public float Rotation { get; set; }
    
    public AlignmentType? Alignment { get; set; } 
    
    protected Element(Element? parent)
    {
        Parent = parent;
        Children = [];
        
        Parent?.Children.Add(this);
    }


    public virtual void Update()
    {
        if (Parent is not null && Alignment is not null)
            AbsolutePosition = AlignmentCalculator.GetAlignedPositionOfChild(GetBounds(), Parent.GetBounds(), Alignment.Value);
     
        UpdateSfmlProperties();
        
        foreach (Element child in Children)
            child.Update();
    }

    
    public virtual void Draw(RenderTarget target)
    {
        foreach (Element child in Children)
            child.Draw(target);
    }


    public abstract FloatRect GetBounds();


    protected virtual void UpdateSfmlProperties()
    {
        Transformable.Position = AbsolutePosition;
        Transformable.Rotation = Rotation;
    }
}