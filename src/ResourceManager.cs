using System;
using System.IO;


namespace Stella;


public static class ResourceManager
{
    public static string ResourcesDirectory { get; private set; }
    public static string TilesSpriteDirectory { get; private set; }
    public static string FontsDirectory { get; private set; }
    
    
    static ResourceManager()
    {
        // set current directory to project directory
        Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../");

        ResourcesDirectory = Path.Combine(Environment.CurrentDirectory, "resources");
        TilesSpriteDirectory = Path.Combine(ResourcesDirectory, "images/tiles");
        FontsDirectory = Path.Combine(ResourcesDirectory, "fonts");
    }


    public static string GetTileDirectoryPath(string tileName)
        => Path.Combine(TilesSpriteDirectory, tileName);
    
    public static string GetFontFilePath(string fontFileName)
        => Path.Combine(FontsDirectory, fontFileName);
}