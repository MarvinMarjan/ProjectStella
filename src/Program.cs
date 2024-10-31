using System.IO;

using SFML.Graphics;


namespace Stella.Game;


class Game
{
    private static void Main(string[] args)
    {
        GameWindow window = new()
        {
            World = new World(new(150, 150), 45)
        };

        TileDrawable drawable = new(Path.Combine(GlobalSettings.TilesSpriteDirectory, "grass/grass1.png"));
        
        window.World.FillAllWith(drawable);
        
        while (window.IsOpen)
        {
            window.Update();
            window.Draw();
        }
    }
}