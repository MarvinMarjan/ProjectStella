using System.IO;

using SFML.Graphics;
using SFML.System;

using Stella.Game;


namespace Stella.UI.Elements;


public class TextElement : Element
{
    public static Font DefaultTextFont = new(Path.Combine(GlobalSettings.FontsDirectory, "itim.ttf"));
    
    public Text Text { get; protected set; }


    public TextElement(Element? parent, Vector2f position, uint size, string text, Font? font = null) : base(parent)
    {
        Text = new(text, font ?? DefaultTextFont)
        {
            Position = position,
            CharacterSize = size
        };
    }
    

    public override void Draw(RenderTarget target)
    {
        target.Draw(Text);
        
        base.Draw(target);
    }
}