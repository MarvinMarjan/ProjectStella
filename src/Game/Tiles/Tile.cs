using SFML.System;


namespace Stella.Game.Tiles;


public class Tile
{
    public Vector2f Position { get; }
    public int Size { get; }

    private TileDrawable? _object;
    public TileDrawable? Object
    {
        get => _object;
        set
        {
            _object = value;

            if (_object is null)
                return;
            
            // change TileDrawable properties to match with this Tile
            MatchTileDrawablePropertiesWithThis(_object);
        }
    }
    
    
    public Tile(Vector2f position, int size, TileDrawable? @object = null)
    {
        Position = position;
        Size = size;
        
        Object = @object;
    }


    public void MatchTileDrawablePropertiesWithThis(TileDrawable tileDrawable)
    {
        tileDrawable.Position = Position;
        tileDrawable.SetScaleToPixels(new(Size, Size));
    }
}