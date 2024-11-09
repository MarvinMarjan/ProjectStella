using System;
using System.Threading;

using SFML.System;
using SFML.Graphics;

using Stella.Game;
using Stella.Game.Tiles;
using Stella.Game.World;
using Stella.UI;
using Stella.UI.Elements;


namespace Stella.Areas;


public class MainGame : Area
{
    public ProgressBarPopup WorldGenerationProgressPopup { get; }
    public TextElement WorldGenerationStage { get; }
    private bool _wasWorldGenerated, _worldGenerated;
    
    public TileWorld World { get; private set; } 
    public Camera? Camera { get; private set; }
    
    public event EventHandler? WorldGenerated;
    
    
    public MainGame(MainWindow window) : base(window)
    {
        WorldGenerationProgressPopup = new("World Generation Progress");
        WorldGenerationProgressPopup.Show();

        WorldGenerationStage = new(WorldGenerationProgressPopup, new(), 30, "Starting")
        {
            Alignment = AlignmentType.Center
        };
        
        _wasWorldGenerated = _worldGenerated = false;
        
        Vector2u worldSize = new(1024, 1024);
        World = new(Window.View, worldSize);

        float[,] noise = WorldGenerator.GenerateWorldNoise(worldSize, null);
        
        new Thread(() => WorldGenerator.FillWorldFromNoiseAsync(World, noise).Wait()).Start();
    }


    private void OnWorldGenerated()
    {
        Camera = new(Window);
        
        World.StartUpdateThreads();
        
        Window.Closed += (_, _) => World.EndUpdateThreads();
        Window.MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;
        
        Window.View.Center = World.Tiles[World.TileCount.X / 2, World.TileCount.Y / 2].Position;
        
        WorldGenerated?.Invoke(this, EventArgs.Empty);
    }

    
    public sealed override void Update()
    {
        Camera?.Update();

        WorldGenerationProgressPopup.ProgressBar.Progress = CalculateWorldGenerationProgress(World);
        
        _worldGenerated = WorldGenerationProgressPopup.ProgressBar.IsAtMax;
        
        if (!_wasWorldGenerated && _worldGenerated)
            OnWorldGenerated();
        
        if (_worldGenerated)
            World.Update();

        WorldGenerationProgressPopup.Update();
        
        _wasWorldGenerated = _worldGenerated;
    }


    public sealed override void Draw(RenderTarget target)
    {
        if (_worldGenerated)
            World.Draw(Window);
        
        WorldGenerationProgressPopup.Draw(target);
    }
    
    
    private float CalculateWorldGenerationProgress(TileWorld world)
    {
        uint worldSize = world.TileCount.X * world.TileCount.Y;
        uint counter = 0;
        
        foreach (Tile tile in world.Tiles)
            if (tile.Object is not null)
                counter++;
        
        return counter / (float)worldSize;
    }
}