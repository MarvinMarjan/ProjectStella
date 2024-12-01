using SFML.Window;
using SFML.Graphics;

using Stella.Areas;


using Window = Latte.Core.Window;


namespace Stella;


public class MainWindow : Window
{
    private Area? _currentArea;

    
    public Area? CurrentArea
    {
        get => _currentArea;
        set
        {
            _currentArea?.Deinitialize();
            _currentArea = value;
            _currentArea?.Initialize();
        }
    }


    public MainWindow() : base(VideoMode.DesktopMode, "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = 0
    })
    {
        SetVerticalSyncEnabled(true);
    }


    public sealed override void Update()
    {
        base.Update();
        
        CurrentArea?.Update();
    }
    
    
    public sealed override void Draw(RenderTarget target)
    {
        base.Draw(target);
        
        CurrentArea?.Draw(target);
    }
}