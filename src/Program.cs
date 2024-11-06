using Stella.Areas;


namespace Stella;


// TODO: someday, add a question in stackoverflow about the problem that happens when this game uses antialiasing:

// The problem is not about using integers in view transformations and is not
// about integers at all (Tested).

class GameProgram
{
    private static void Main(string[] args)
    {
        MainWindow window = new();
        window.CurrentArea = new MainMenu(window);

        while (window.IsOpen)
        {
            window.Update();
            window.Draw();
        }
    }
}