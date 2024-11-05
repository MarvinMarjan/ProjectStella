using System;
using System.Diagnostics;

using Stella.Game;
using Stella.Game.World;


namespace Stella;


// TODO: someday, add a question in stackoverflow about the problem that happens when this game uses antialiasing:

// The problem is not about using integers in view transformations and is not
// about integers at all (Tested).

class GameProgram
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Started.");
        
        GameWindow window = new();
        
        Stopwatch timer = new();
        timer.Start();
        
        Console.WriteLine("Starting world generation.");
        
        window.World = WorldGenerator.GenerateWorld(window, new(2048, 2048), null);
        window.World.StartUpdateThreads();

        window.View.Center = window.World.Tiles[window.World.Size.X / 2, window.World.Size.Y / 2].Position;
        
        Console.WriteLine($"World generated after {timer.Elapsed.TotalSeconds:F3} seconds.");
        
        while (window.IsOpen)
        {
            window.Update();
            window.Draw();
        }
        
        window.World.EndUpdateThreads();
        
        Console.WriteLine("Bye bye.");
    }
}