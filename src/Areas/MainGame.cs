using SFML.Graphics;

using Stella.Game;
using Stella.Game.World;


namespace Stella.Areas;


public class MainGame : Area
{
    public TileWorld? World { get; set; } 
    public Camera Camera { get; }
    
    public View View { get; set; }

    
    public MainGame(MainWindow window) : base(window)
    {
        View = Window.GetView();

        Camera = new(Window);
        
        World = WorldGenerator.GenerateWorld(View, new(1024, 1024), null);
        World.StartUpdateThreads();
        
        Window.Closed += (_, _) => World.EndUpdateThreads();
        Window.MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;
        
        window.View.Center = World.Tiles[World.Size.X / 2, World.Size.Y / 2].Position;
    }

    
    public sealed override void Update()
    {
        Camera.Update();
        World?.Update();
    }


    public sealed override void Draw(RenderTarget target)
    {
        World?.Draw(Window);
    }
}