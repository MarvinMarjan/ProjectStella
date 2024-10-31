using Stella.Game;
using Stella.Game.World;


namespace Stella;


class GameProgram
{
    private static void Main(string[] args)
    {
        GameWindow window = new()
        {
            World = new WorldGeneration(10).GenerateWorld(new(500, 500))
        };
        
        while (window.IsOpen)
        {
            window.Update();
            window.Draw();
        }
    }
}