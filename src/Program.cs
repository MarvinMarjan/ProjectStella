using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using Stella.Game;
using Stella.Game.Tiles;
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

        Vector2u worldSize = new(1000, 1000);
        float[,] noise = new WorldGenerator(new Random().Next()).GenerateNoise(worldSize);

        window.World = new TileWorld(window, worldSize);
        
        WorldGenerator.FillWorldFromNoiseAsync(window.World, noise).Wait();
        window.World.MinimizedVerticesUpdateRequested = true;
        
        window.World.StartUpdateThreads();
        
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