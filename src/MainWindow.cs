using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Stella.Areas;
using Stella.Game;


namespace Stella;


public class MainWindow : RenderWindow
{
    public Area? CurrentArea { get; set; }
    
    public View View { get; set; }
    
    
    public MainWindow() : base(VideoMode.FullscreenModes[0], "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = GlobalSettings.AntialiasingLevel
    })
    {
        View = GetView();
     
        Closed += (_, _) => Close();
        Resized += (_, args) => ResizeViewToFitScreenSize(new(args.Width, args.Height));
        
        SetVerticalSyncEnabled(true);
    }


    public void Update()
    {
        DispatchEvents();
        SetView(View);

        CurrentArea?.Update();
    }
    
    
    public void Draw()
    {
        Clear();
        CurrentArea?.Draw(this);
        Display();
    }
    
    
    private void ResizeViewToFitScreenSize(Vector2u newSize)
    {
        View.Size = (Vector2f)newSize;
    }
}