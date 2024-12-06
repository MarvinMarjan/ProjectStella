using SFML.Window;


using Window = Latte.Core.Window;


namespace Stella;


public class MainWindow : Window
{
    public MainWindow() : base(VideoMode.DesktopMode, "Project Stella", Styles.Default, new()
    {
        AntialiasingLevel = 0
    })
    {
        SetVerticalSyncEnabled(true);
    }
}