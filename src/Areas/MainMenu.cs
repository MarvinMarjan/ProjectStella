using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;

using Stella.Game.World;


namespace Stella.Areas;


public class MainMenu : Area
{
    public TileWorld BackgroundWorld { get; }
    
    public RectangleElement MenuBackground { get; }
    public ButtonElement PlayButton { get; }
    public ButtonElement ExitButton { get; }
    
    
    public MainMenu(MainWindow window) : base(window)
    {
        BackgroundWorld = WorldGenerator.GenerateWorld(App.MainView, new(128, 128));
        BackgroundWorld.StartUpdateThreads();
        
        MenuBackground = new(null, new(), new(220f, 300f))
        {
            Alignment = { Value = Alignments.Center },
            AlignmentMargin = { Value = new(0f, 200f) },
            Color = { Value = new(50, 50, 50, 150) }
        };
        
        PlayButton = new(MenuBackground, new(), new(160f, 30f), "Play")
        {
            BorderColor = { Value = Color.Black },
            BorderSize = { Value = 2f },
            Alignment = { Value = Alignments.HorizontalCenter | Alignments.Top },
            AlignmentMargin = { Value = new(0f, 20f) }
        };

        ExitButton = new (MenuBackground, new(), new(160, 30f), "Exit")
        {
            BorderColor = { Value = Color.Black },
            BorderSize = { Value = 2f },
            Alignment = { Value = Alignments.HorizontalCenter | Alignments.Top },
            AlignmentMargin = { Value = new(0f, 70f) }
        };

        PlayButton.MouseUpEvent += (_, _) => Window.CurrentArea = new MainGame(Window);
        ExitButton.MouseUpEvent += (_, _) => Window.Close();
    }
    

    public sealed override void Deinitialize()
    {
        base.Deinitialize();
        
        App.RemoveElement(MenuBackground);    
    }
    
    
    public sealed override void Update()
    {
        BackgroundWorld.Update();
    }

    
    public sealed override void Draw(RenderTarget target)
    {
        BackgroundWorld.Draw(target);
    }
}