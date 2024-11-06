using System;
using System.IO;


namespace Stella.Game;


public static class GlobalSettings
{
    public static uint AntialiasingLevel { get; set; } = 0;
    
    public static string? GeneratedPerlinSavePath => Path.Combine(Environment.CurrentDirectory, "test.png");
    
    public static string ResourcesDirectory { get; private set; }
    public static string TilesSpriteDirectory { get; private set; }
    public static string FontsDirectory { get; private set; }
    

    static GlobalSettings()
    {
        // set current directory to project directory
        Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../");

        ResourcesDirectory = Path.Combine(Environment.CurrentDirectory, "resources");
        TilesSpriteDirectory = Path.Combine(ResourcesDirectory, "images/tiles");
        FontsDirectory = Path.Combine(ResourcesDirectory, "fonts");
    }
}