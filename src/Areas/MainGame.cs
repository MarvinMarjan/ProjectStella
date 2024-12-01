using SFML.System;
using SFML.Graphics;

using Latte.Core.Application;

using Stella.Game;
using Stella.Game.Tiles;
using Stella.Game.World;


namespace Stella.Areas;


// TODO: add ability to change the tiles in the world.
// TODO: add world saving. save seed and tiles changed by the player; use binary files to store it
// TODO: add interface (GUI) to create, select and remove worlds.
// TODO: add animations to the tiles
// TODO: add lighting (day cycle maybe?)


public class MainGame : Area
{
    public TileWorld World { get; }

    public Camera Camera { get; private set; }
    
    
    public MainGame(MainWindow window, TileWorld world) : base(window)
    {
        World = world;
        World.StartUpdateThreads();
        
        Camera = new(App.Window, App.MainView);
        Camera.BoundsLimit = new(World.Tiles[0, 0].Position, (Vector2f)World.TileCount * TileDrawable.DefaultTilePixelSize);
    }

    
    public sealed override void Update()
    {
        Camera.Update();
        World.Update();
    }
    

    public sealed override void Draw(RenderTarget target)
    {
        World.Draw(Window);
    }
}