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
    public ProgressBarPopup? WorldGenerationProgressPopup { get; private set; }
    public TextElement WorldGenerationStage { get; }
    
    public TileWorld World { get; private set; } 
    public Camera? Camera { get; private set; }
    
    private bool _worldGenerated;
    
    
    public MainGame(MainWindow window) : base(window)
    {
        WorldGenerationProgressPopup = new("World Generation Progress");
        WorldGenerationProgressPopup.Show();

        WorldGenerationProgressPopup.UpdateEvent += (_, _) =>
            WorldGenerationProgressPopup.ProgressBar.Progress = CalculateWorldGenerationProgress(World!);

        WorldGenerationProgressPopup.ClosedEvent += (_, _) =>
        {
            WorldGenerationProgressPopup = null;
            OnWorldGenerated();
        };
        
        WorldGenerationStage = new(WorldGenerationProgressPopup, new(), 30, "Starting")
        {
            Alignment = AlignmentType.Center
        };
        
        Vector2u worldSize = new(1024, 1024);
        World = new(Window.View, worldSize);

        float[,] noise = WorldGenerator.GenerateWorldNoise(worldSize, null);
        
        new Thread(() => WorldGenerator.FillWorldFromNoiseAsync(World, noise).Wait()).Start();
    }


    private void OnWorldGenerated()
    {
        _worldGenerated = true;
        
        Camera = new(Window);
        
        World.StartUpdateThreads();
        
        Window.Closed += (_, _) => World.EndUpdateThreads();
        Window.MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;

        Window.View.Center = World.GetCenterPosition();
    }

    
    public sealed override void Update()
    {
        Camera?.Update();
        WorldGenerationProgressPopup?.Update();
        
        if (_worldGenerated)
            World.Update();
    }


    public sealed override void Draw(RenderTarget target)
    {
        if (_worldGenerated)
            World.Draw(Window);
        
        WorldGenerationProgressPopup?.Draw(target);
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