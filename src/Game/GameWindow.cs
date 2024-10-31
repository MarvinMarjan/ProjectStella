using SFML.System;
using SFML.Window;
using SFML.Graphics;


namespace Stella.Game;


/// <summary>
/// The game main window.
/// </summary>
public class GameWindow : RenderWindow
{
    public required World World { get; set; }
    public Camera Camera { get; }
    
    public View CurrentView { get; set; }
    
    
    public GameWindow() : base(VideoMode.FullscreenModes[0], "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = 2
    })
    {
        SetVerticalSyncEnabled(true);

        Camera = new(this);

        CurrentView = GetView();
        
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;
    }

    
    public void Update()
    {
        DispatchEvents();
        
        Camera.Update();
        SetView(CurrentView);
    }


    public void Draw()
    {
        Clear();
        
        World.DrawAll(this);
        
        Display();
    }


    private void ResizeViewToFitScreenSize(Vector2u newSize)
    {
        View view = GetView();
        view.Size = (Vector2f)newSize;
        
        SetView(view);
    }
}