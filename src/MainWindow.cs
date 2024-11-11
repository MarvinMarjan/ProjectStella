using System;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using Stella.Areas;


namespace Stella;


public class MainWindow : RenderWindow
{
    public static MainWindow? Current { get; private set; }
    
    public Vector2i MousePosition => Mouse.GetPosition(this);
    public Vector2f WorldMousePosition => MapPixelToCoords(MousePosition);
    
    public Area? CurrentArea { get; set; }
    
    public View View { get; set; }

    public event EventHandler? ClosedEvent;
    
    
    public MainWindow() : base(VideoMode.DesktopMode, "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = 0
    })
    {
        Current = this;
        
        View = GetView();
     
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        
        SetVerticalSyncEnabled(true);
    }


    public void Update()
    {
        DispatchEvents();
        
        CurrentArea?.Update();

        SetView(View); // update view after area
    }
    
    
    public void Draw()
    {
        Clear();
        CurrentArea?.Draw(this);
        Display();
    }


    public override void Close()
    {
        ClosedEvent?.Invoke(this, EventArgs.Empty);
        base.Close();
        
        Environment.Exit(0); // force exit and any thread alive to abort
    }
    
    
    private void ResizeViewToFitScreenSize(Vector2u newSize)
    {
        View.Size = (Vector2f)newSize;
    }
}