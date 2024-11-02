using System;
using System.Collections.Generic;

using SFML.Graphics;


namespace Stella.Game.Tiles;


public class TileSet : Image
{
    public uint Width { get; }
    public uint Height { get; }
    public uint PixelWidth => Width * TileSize;
    public uint PixelHeight => Height * TileSize;
    public uint TileSize { get; }

    private uint _currentPixelX;
    private uint _currentPixelY;
    private uint _minPixelX;
    private uint _minPixelY;
    
    public bool IsFull { get; private set; }


    public TileSet(uint width, uint height, uint tileSize) : base(width * tileSize, height * tileSize)
    {
        Width = width;
        Height = height;
        TileSize = tileSize;

        _currentPixelX = _currentPixelY = 0;
        _minPixelX = _minPixelY = 0;
    }
    

    public void LoadFromTileIndex()
    {
        List<Image[]> imageMatrix = [];
        
        foreach (var (_, images) in TileIndex.LoadedTiles)
            imageMatrix.Add(images);

        foreach (Image[] images in imageMatrix)
            AddImageArray(images);
    }


    private void AddImageArray(Image[] images)
    {
        foreach (Image image in images)
            AddImage(image);
    }


    private void AddImage(Image image)
    {
        if (image.Size.X != TileSize || image.Size.Y != TileSize)
            throw new ArgumentException($"Image width and height must be {TileSize}.");

        _currentPixelX = _minPixelX;
        _currentPixelY = _minPixelY;
        
        for (uint row = 0; row < TileSize; row++, _currentPixelY++)
        {
            for (uint col = 0; col < TileSize; col++, _currentPixelX++)
                SetPixel(_currentPixelX, _currentPixelY, image.GetPixel(col, row));
            
            if (row + 1 < TileSize)
                _currentPixelX = _minPixelX;
        }
        
        _minPixelX += TileSize;

        if (_minPixelX >= PixelWidth)
        {
            _minPixelY += TileSize;
            _minPixelX = 0;
        }

        if (_minPixelY >= PixelHeight)
            IsFull = true;
    }
}