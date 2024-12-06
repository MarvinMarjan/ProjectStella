using SFML.Graphics;

using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;

using Stella.Game.World;
using Stella.GUI.MainMenu;


namespace Stella.Sections;


public class MainMenu : Section
{
    public TileWorld BackgroundWorld { get; }
    
    public TileWorld? World => WorldGenerator.TileWorld;
    public WorldGenerator WorldGenerator { get; }
    
    public RectangleElement MenuBackground { get; }
    public GridLayoutElement MenuGrid { get; }
    public MenuButton PlayButton { get; }
    public MenuButton ExitButton { get; }
    
    
    public MainMenu()
    {
        BackgroundWorld = WorldGenerator.GenerateWorld(App.MainView, new(128, 128));
        BackgroundWorld.StartUpdateThreads();

        WorldGenerator = new(App.MainView, new(64 * 20, 64 * 20));
        
        MenuBackground = new(null, new(), new(300f))
        {
            Alignment = { Value = Alignments.VerticalCenter | Alignments.Left },
            AlignmentMargin = { Value = new(220f) },
            
            BorderSize = { Value = 2f },
            BorderColor = { Value = new(0, 0, 0, 180) },
            Color = { Value = new(0, 0, 0, 120) }
        };

        MenuBackground.UpdateEvent += (_, _) => MenuBackground.Size.Value.Y = App.Window.Size.Y;

        MenuGrid = new(MenuBackground, new(), 2, 1, 300f, 70f)
        {
            Alignment = { Value = Alignments.Center }
        };

        PlayButton = new MenuButton("Play");
        ExitButton = new MenuButton("Exit");

        MenuGrid.AddElement(PlayButton);
        MenuGrid.AddElement(ExitButton);
        
        PlayButton.MouseUpEvent += (_, _) => App.Section = new WorldMenu(BackgroundWorld);
        ExitButton.MouseUpEvent += (_, _) => App.Window.Close();
        
        AddElement(MenuBackground);
    }
    
    
    public sealed override void Update()
    {
        base.Update();
        
        BackgroundWorld.Update();
    }

    
    public sealed override void Draw(RenderTarget target)
    {
        base.Draw(target);
        
        BackgroundWorld.Draw(target);
    }
}