using SFML.System;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives.Shapes;

using Stella.GUI.MainMenu;
using Stella.Game.World;


namespace Stella.Areas;


public class MainMenu : Area
{
    public TileWorld BackgroundWorld { get; }
    
    public TileWorld? World => WorldGenerator.TileWorld;
    public WorldGenerator WorldGenerator { get; }
    
    public RectangleElement MenuBackground { get; }
    public GridLayout MenuGrid { get; }
    public MenuButton PlayButton { get; }
    public MenuButton ExitButton { get; }
    
    // public WorldGenerationProgressPopup WorldGenerationProgressPopup { get; }
    
    
    public MainMenu(MainWindow window) : base(window)
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
        
        PlayButton.MouseUpEvent += (_, _) => Window.CurrentArea = new WorldMenu(Window, BackgroundWorld);
        ExitButton.MouseUpEvent += (_, _) => Window.Close();
        
        // WorldGenerationProgressPopup = new WorldGenerationProgressPopup(WorldGenerator);
        // WorldGenerationProgressPopup.Close();
        //
        // WorldGenerationProgressPopup.CloseEvent += (_, _) => Window.CurrentArea = new MainGame(Window, World!);
    }
    

    public sealed override void Deinitialize()
    {
        base.Deinitialize();
        
        App.RemoveElement(MenuBackground);
        // App.RemoveElement(WorldGenerationProgressPopup);
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