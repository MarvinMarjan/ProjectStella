using System.IO;
using System.Collections.Generic;

using SFML.Graphics;

using Stella.Game.World;


namespace Stella.Game.Tiles;


public static class TileIndex
{
    public static readonly Dictionary<string, TileDrawable> Tiles = new([
        new("grass", new TileDrawable(TileTextureFromPath("grass/grass1.png"))),
        new("water", new TileDrawable(TileTextureFromPath("water/water1.png"))),
        new("stone", new TileDrawable(TileTextureFromPath("stone/stone1.png")))
    ]);

    public static readonly Dictionary<NoiseRange, string> TileNoiseRange = new([
        new(new(0f, 0.4f), "water"),
        new(new(0.4f, 1.7f), "grass"),
        new(new(1.7f, 2f), "stone")
    ]);


    public static Texture TileTextureFromPath(string path)
        => new(Path.Combine(GlobalSettings.TilesSpriteDirectory, path));


    public static TileDrawable? FromNoiseValue(float noiseValue)
    {
        foreach (var (range, value) in TileNoiseRange)
            if (range.IsBetween(noiseValue))
                return Tiles[value].Clone() as TileDrawable;

        return null;
    }
}