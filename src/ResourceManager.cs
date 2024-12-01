using System;
using System.IO;


namespace Stella;


public static class ResourceManager
{
    public static string ResourcesDirectory { get; }
    public static string TilesSpriteDirectory { get; }
    public static string FontsDirectory { get; }
    
    
    static ResourceManager()
    {
        // set current directory to project directory
        Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../");

        ResourcesDirectory = Path.Combine(Environment.CurrentDirectory, "resources");
        TilesSpriteDirectory = Path.Combine(ResourcesDirectory, "images/tiles");
        FontsDirectory = Path.Combine(ResourcesDirectory, "fonts");
    }


    public static string GetTileDirectoryPath(string tileFileName)
        => Path.Combine(TilesSpriteDirectory, tileFileName);
    
    public static string GetFontFilePath(string fontFileName)
        => Path.Combine(FontsDirectory, fontFileName);
}