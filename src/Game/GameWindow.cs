using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Stella.Game.World;


namespace Stella.Game;


/// <summary>
/// The game main window.
/// </summary>
public class GameWindow : RenderWindow
{
    public TileWorld? World { get; set; } 
    public Camera Camera { get; }
    
    public View View { get; set; }
    
    
    public GameWindow() : base(VideoMode.FullscreenModes[0], "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = GlobalSettings.AntialiasingLevel
    })
    {
        SetVerticalSyncEnabled(true);
        
        View = GetView();

        Camera = new(this);
        
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;
    }

    
    public void Update()
    {
        DispatchEvents();
        
        Camera.Update();
        SetView(View);
        
        World?.Update(this);
    }


    public void Draw()
    {
        Clear();
        
        World?.Draw(this);
        
        Display();
    }


    private void ResizeViewToFitScreenSize(Vector2u newSize)
    {
        View view = GetView();
        view.Size = (Vector2f)newSize;
        
        SetView(view);
    }
}