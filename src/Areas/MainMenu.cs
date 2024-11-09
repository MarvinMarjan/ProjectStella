using SFML.Graphics;

using Stella.Game.World;
using Stella.UI;
using Stella.UI.Elements;
using Stella.UI.Elements.Shapes;


namespace Stella.Areas;


public class MainMenu : Area
{
    public TileWorld BackgroundWorld { get; }
    
    public RectangleElement MenuBackground { get; }
    public ButtonElement PlayButton { get; }
    
    
    public MainMenu(MainWindow window) : base(window)
    {
        BackgroundWorld = WorldGenerator.GenerateWorld(Window.View, new(128, 128), null);
        BackgroundWorld.StartUpdateThreads();
        
        // TODO: TileWorld.GetCenterPosition()
        Window.View.Center = BackgroundWorld.Tiles[64, 64].Position;
        Window.Closed += (_, _) => BackgroundWorld.EndUpdateThreads();
        
        MenuBackground = new(null, new(), new(220f, 300f))
        {
            Alignment = AlignmentType.Center,
            AlignmentMargin = new(0f, 200f),
            Color = new(50, 50, 50, 150)
        };
        
        PlayButton = new(MenuBackground, new(), new(160f, 30f), "Play")
        {
            BorderColor = Color.Black,
            BorderSize = 2f,
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top,
            AlignmentMargin = new(0f, 20f)
        };

        PlayButton.MouseUpEvent += (_, _) => Window.CurrentArea = new MainGame(Window);
    }
    

    public sealed override void Update()
    {
        BackgroundWorld.Update();
        MenuBackground.Update();
    }

    
    public sealed override void Draw(RenderTarget target)
    {
        BackgroundWorld.Draw(target);
        MenuBackground.Draw(target);
    }
}