using System;

using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileTextureAnimation
{
    public const uint MaxTextureCount = 4;
    
    public Texture[] Textures { get; }


    public TileTextureAnimation(Texture[] textures)
    {
        if (textures.Length > MaxTextureCount)
            throw new ArgumentException($"Max animation texture count is {MaxTextureCount}, got {textures.Length}.");
        
        Textures = textures;
    }

    public TileTextureAnimation(Image[] images)
    {
        Texture[] textures = new Texture[MaxTextureCount];

        for (int i = 0; i < images.Length && i < MaxTextureCount; i++)
            textures[i] = new(images[i]);

        Textures = textures;
    }


    public Texture this[int index]
    {
        get => Textures[index];
        set => Textures[index] = value;
    }
}