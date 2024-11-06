using SFML.Graphics;
using Stella.UI;
using Stella.UI.Elements;
using Stella.UI.Elements.Shapes;


namespace Stella.Areas;


public class MainMenu : Area
{
    public RectangleElement Parent { get; set; }
    public RectangleElement Child { get; set; }
    
    public TextElement Text { get; set; }
    
    
    public MainMenu(MainWindow window) : base(window)
    {
        Parent = new(null, new(300, 300), new(400, 400));
        Child = new(Parent, new(), new(100, 100))
        {
            Alignment = AlignmentType.VerticalCenter | AlignmentType.Right,
            Color = Color.Red
        };
        
        Text = new(null, new(300, 300), 60, "Hello, World!");
    }
    

    public sealed override void Update()
    {
        Text.Update();
        
        Parent.Update();
    }

    
    public sealed override void Draw(RenderTarget target)
    {
        Text.Draw(Window);
        
        Parent.Draw(Window);
    }
}