using SFML.System;
using SFML.Graphics;


namespace Stella.UI.Elements;


public class TextElement : Element
{
    public static Font DefaultTextFont = new(ResourceManager.GetFontFilePath("itim.ttf"));
    
    public override Transformable Transformable => Text;
    
    public Text Text { get; protected set; }
    

    public TextElement(Element? parent, Vector2f position, uint size, string text, Font? font = null) : base(parent)
    {
        Text = new(text, font ?? DefaultTextFont)
        {
            CharacterSize = size
        };
        
        Position = position;
    }
    

    public override void Draw(RenderTarget target)
    {
        target.Draw(Text);
        
        base.Draw(target);
    }
    
    
    public override FloatRect GetBounds()
        => Text.GetGlobalBounds();


    public override Vector2f GetAlignmentPosition(AlignmentType alignment)
    {
        // text local bounds work quite different
        // https://learnsfml.com/basics/graphics/how-to-center-text/#set-a-string
        
        FloatRect localBounds = Text.GetLocalBounds();
        
        Vector2f position = base.GetAlignmentPosition(alignment);
        position -= localBounds.Position;
        
        return position;
    }
}