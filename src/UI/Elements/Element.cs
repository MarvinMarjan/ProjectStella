using System.Collections.Generic;

using SFML.Graphics;


namespace Stella.UI.Elements;


public abstract class Element : IUpdateable, IDrawable
{
    public Element? Parent { get; set; }
    public List<Element> Children { get; }
    
    
    protected Element(Element? parent)
    {
        Parent = parent;
        Children = [];
        
        Parent?.Children.Add(this);
    }


    public virtual void Update()
    {
        foreach (Element child in Children)
            child.Update();
    }

    
    public virtual void Draw(RenderTarget target)
    {
        foreach (Element child in Children)
            child.Draw(target);
    }
}