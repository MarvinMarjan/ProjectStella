using System;
using System.IO;
using System.Collections.Generic;

using SFML.Graphics;

using Stella.Game.World;


namespace Stella.Game.Tiles;


public static class TileIndex
{
    // all loaded tile textures
    public static readonly Dictionary<string, Image[]> LoadedTiles = new([
        new("grass", ImageArrayFromTilesDirectory("grass")),
        new("light_grass", ImageArrayFromTilesDirectory("light_grass")),
        new("dirt", ImageArrayFromTilesDirectory("dirt")),
        new("stone", ImageArrayFromTilesDirectory("stone")),
        new("dark_stone", ImageArrayFromTilesDirectory("dark_stone")),
        new("water", ImageArrayFromTilesDirectory("water")),
        new("deep_water", ImageArrayFromTilesDirectory("deep_water")),
        new("sand", ImageArrayFromTilesDirectory("sand")),
        new("snow", ImageArrayFromTilesDirectory("snow"))
    ]);
    
    public static readonly Dictionary<string, NoiseRange> TileNoiseRange = new([
        new("deep_water", new(0f, 0.5f)),
        new("water", new(0.5f, 0.6f)),
        new("sand", new(0.6f, 0.7f)),
        new("light_grass", new(0.7f, 1.0f)),
        new("grass", new(1.0f, 1.3f)),
        new("dirt", new(1.3f, 1.4f)),
        new("stone", new(1.4f, 1.55f)),
        new("dark_stone", new(1.55f, 1.7f)),
        new("snow", new(1.7f, 2f)),
    ]);

    public static readonly Dictionary<string, Color> TileColor = new([
        new("grass", new(10, 135, 0)),
        new("light_grass", new(10, 170, 0)),
        new("dirt", new(95, 70, 30)),
        new("stone", new(150, 150, 150)),
        new("dark_stone", new(120, 120, 120)),
        new("water", new(200, 255, 255)),
        new("deep_water", new(130, 255, 255)),
        new("sand", new(240, 250, 125)),
        new("snow", new(220, 255, 250)),
    ]);


    public static Image[] ImageArrayFromTilesDirectory(string dirName)
    {
        string[] files = Directory.GetFiles(Path.Combine(GlobalSettings.TilesSpriteDirectory, dirName));
        Image[] images = new Image[files.Length];

        for (int i = 0; i < files.Length; i++)
            images[i] = new(files[i]);

        if (files.Length == 0)
            throw new InvalidOperationException("Tile directory must have at least one image.");
        
        return images;
    }


    public static TileDrawable? FromNoiseValue(float noiseValue)
    {
        foreach (var (name, range) in TileNoiseRange)
            if (range.IsBetween(noiseValue))
                return new(name);

        return null;
    }
}