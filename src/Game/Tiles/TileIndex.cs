using System.IO;
using System.Collections.Generic;

using SFML.Graphics;

using Stella.Game.World;


namespace Stella.Game.Tiles;


public static class TileIndex
{
    public static readonly Dictionary<string, TileDrawable> Tiles = new([
        new("grass", new TileDrawable(TileTextureFromPath("grass/grass1.png"))),
        new("dirt", new TileDrawable(TileTextureFromPath("dirt/dirt1.png"))),
        new("stone", new TileDrawable(TileTextureFromPath("stone/stone1.png"))),
        new("deep_water", new TileDrawable(TileTextureFromPath("deep_water/deep_water1.png"))),
        new("water", new TileDrawable(TileTextureFromPath("water/water1.png"))),
        new("sand", new TileDrawable(TileTextureFromPath("sand/sand1.png"))),
        new("snow", new TileDrawable(TileTextureFromPath("snow/snow1.png"))),
    ]);

    public static readonly Dictionary<NoiseRange, string> TileNoiseRange = new([
        new(new(0f, 0.5f), "deep_water"),
        new(new(0.5f, 0.6f), "water"),
        new(new(0.6f, 0.7f), "sand"),
        new(new(0.7f, 1.3f), "grass"),
        new(new(1.3f, 1.4f), "dirt"),
        new(new(1.4f, 1.8f), "stone"),
        new(new(1.8f, 2f), "snow"),
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