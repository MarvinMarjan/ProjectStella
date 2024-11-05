using System;
using System.IO;
using System.Collections.Generic;

using SFML.Graphics;

using Stella.Game.World;


namespace Stella.Game.Tiles;


public readonly struct TileIndexData
{
    public Image[] Images { get; }
    public TileTextureAnimation TextureAnimation { get; }
    public NoiseRange NoiseRange { get; }
    public Color Color { get; }
    
    
    public TileIndexData(Image[] images, NoiseRange noiseRange, Color color)
    {
        Images = images;
        TextureAnimation = new(Images);
        NoiseRange = noiseRange;
        Color = color;
    }
}


public static class TileIndex
{
    // all loaded tile textures
    public static readonly Dictionary<string, TileIndexData> LoadedTiles = new([
        NewTileIndexData("grass", new(0.7f, 1.0f), new(10, 170, 0)),
        NewTileIndexData("dark_grass", new(1.0f, 1.3f), new(10, 135, 0)),
        NewTileIndexData("dirt", new(1.3f, 1.4f), new(95, 70, 30)),
        NewTileIndexData("stone", new(1.4f, 1.55f), new(150, 150, 150)),
        NewTileIndexData("dark_stone", new(1.55f, 1.7f), new(120, 120, 120)),
        NewTileIndexData("water", new(0.5f, 0.6f), new(0, 203, 255)),
        NewTileIndexData("deep_water", new(0f, 0.5f), new(0, 152, 255)),
        NewTileIndexData("sand", new(0.6f, 0.7f), new(240, 250, 125)),
        NewTileIndexData("snow", new(1.7f, 2f), new(220, 255, 250))
    ]);


    private static KeyValuePair<string, TileIndexData> NewTileIndexData(string name, NoiseRange noiseRange, Color color)
        => new(name, new(ImageArrayFromTilesDirectory(name), noiseRange, color));


    public static int GetTileIndexByName(string name)
    {
        int index = 0;

        foreach (var (tileName, _) in LoadedTiles)
        {
            if (tileName == name)
                break;

            index++;
        }

        return index;
    }


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
        foreach (var (name, tile) in LoadedTiles)
            if (tile.NoiseRange.IsBetween(noiseValue))
                return new(name);

        return null;
    }
}