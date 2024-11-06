using SFML.Graphics;

using Stella.UI.Elements;


namespace Stella.Areas;


public class MainMenu : Area
{
    public TextElement Text { get; set; }
    
    
    public MainMenu(MainWindow window) : base(window)
    {
        Text = new(null, new(300, 300), 60, "Hello, World!");
    }
    

    public sealed override void Update()
    {
        
    }

    
    public sealed override void Draw(RenderTarget target)
    {
        Text.Draw(Window);
    }
}