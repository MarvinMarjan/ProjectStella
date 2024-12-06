using Latte.Core.Application;

using Stella.Sections;


namespace Stella;


// TODO: someday, add a question in stackoverflow about the problem that happens when this game uses antialiasing:

// The problem is not about using integers in view transformations and is not
// about integers at all (Tested).

class GameProgram
{
    private static void Main()
    {
        App.Init(new(ResourceManager.GetFontFilePath("space-grotesk.ttf")));
        App.InitWindow(new MainWindow());
        
        App.Section = new MainMenu();

        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}