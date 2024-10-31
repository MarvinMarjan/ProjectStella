using System;
using System.IO;


namespace Stella.Game;


public static class GlobalSettings
{
    public static string ResourcesDirectory { get; private set; }
    public static string TilesSpriteDirectory { get; private set; }


    static GlobalSettings()
    {
        // set current directory to project directory
        Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../../");

        ResourcesDirectory = Path.Combine(Environment.CurrentDirectory, "resources");
        TilesSpriteDirectory = Path.Combine(ResourcesDirectory, "images/tiles");
    }
}