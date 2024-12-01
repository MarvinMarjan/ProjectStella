using Latte.Core.Application;

using Stella.Areas;


namespace Stella;


// TODO: someday, add a question in stackoverflow about the problem that happens when this game uses antialiasing:

// The problem is not about using integers in view transformations and is not
// about integers at all (Tested).

class GameProgram
{
    private static void Main(string[] args)
    {
        App.Init(new(ResourceManager.GetFontFilePath("itim.ttf")));
        App.InitWindow(new MainWindow());
        
        MainWindow mainWindow = (App.Window as MainWindow)!;
        mainWindow.CurrentArea = new MainMenu(mainWindow);

        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}