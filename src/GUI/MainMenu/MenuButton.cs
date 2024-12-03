using Latte.Core;
using Latte.Core.Type;


namespace Stella.GUI.MainMenu;


public class MenuButton : Button
{
    public MenuButton(string text) : base(null, new(), new(300f, 70f), text)
    {
        Alignment.Set(Alignments.Center);
        
        Radius.Set(0f);
        
        Text.Color.Set(SFML.Graphics.Color.White);
        Color.Set(SFML.Graphics.Color.Transparent);
        
        BorderSize.Set(0f);
        BorderColor.Set(SFML.Graphics.Color.Transparent);
        
        Text.Size.Set(35);
        
        Hover = new()
        {
            { "Color", new ColorRGBA(0, 0, 0, 80) }
        };
        
        Down = new()
        {
            { "Color", new ColorRGBA(0, 0, 0, 160) }
        };
    }
}