using System;
using System.Diagnostics;

using Stella.Game;
using Stella.Game.World;


namespace Stella;


class GameProgram
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Started.");
        
        GameWindow window = new();
        
        Stopwatch timer = new();
        timer.Start();
        
        Console.WriteLine("Starting world generation.");
        
        window.World = WorldGenerator.GenerateWorld(window, new(2304, 2304), null);
        window.World.StartUpdateThreads();

        window.View.Center = window.World.Tiles[0, 0].Position;
        
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