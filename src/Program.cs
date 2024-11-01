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
        
        window.World = new WorldGeneration(100).GenerateWorld(window, new(500, 500));
        window.World.StartUpdateThread();
        
        Console.WriteLine($"World generated after {timer.Elapsed.TotalSeconds:F3} seconds.");
        
        while (window.IsOpen)
        {
            window.Update();
            window.Draw();
        }
        
        window.World.EndUpdateThread();
        
        Console.WriteLine("Bye bye.");
    }
}