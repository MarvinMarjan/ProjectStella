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

        Vector2u worldSize = new(512, 512);
        float[,] noise = new WorldGenerator(new Random().Next()).GenerateNoise(worldSize);

        window.World = new TileWorld(window, worldSize);
        window.World.StartUpdateThreads();
        
        WorldGenerator.FillWorldFromNoiseAsync(window.World, noise).Wait();
        
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