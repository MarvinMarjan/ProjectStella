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
    public TextElement WorldGenerationStageText { get; }

    private WorldGenerator _worldGenerator;
    public TileWorld? World => _worldGenerator.TileWorld;
    private bool _worldGenerated;
    
    public Camera? Camera { get; private set; }
    
    
    public MainGame(MainWindow window) : base(window)
    {
        _worldGenerator = new(Window.View, new(64 * 35, 64 * 35));
        _worldGenerator.StartWorldGeneration();
        
        WorldGenerationProgressPopup = new("World Generation Progress");
        WorldGenerationProgressPopup.Show();

        WorldGenerationProgressPopup.UpdateEvent += (_, _) => OnWorldProgressPopupUpdated();
        WorldGenerationProgressPopup.ClosedEvent += (_, _) => OnWorldGenerated();
        
        WorldGenerationStageText = new(WorldGenerationProgressPopup, new(), 30, "Starting")
        {
            Alignment = AlignmentType.Center
        };
    }

    
    public sealed override void Update()
    {
        if (_worldGenerated)
            World?.Update();

        Camera?.Update();
        WorldGenerationProgressPopup?.Update();
    }


    public sealed override void Draw(RenderTarget target)
    {
        if (_worldGenerated)
            World?.Draw(Window);
        
        WorldGenerationProgressPopup?.Draw(target);
    }
    
    
    private void OnWorldProgressPopupUpdated()
    {
        WorldGenerationStageText.Text.DisplayedString = WorldGenerationStageToString(_worldGenerator.Stage);
            
        if (World is not null)
            WorldGenerationProgressPopup!.ProgressBar.Progress = CalculateWorldGenerationProgress(World);
    }


    private void OnWorldGenerated()
    {
        if (World is null)
            return;
        
        WorldGenerationProgressPopup = null;
        
        _worldGenerated = true;
        
        Camera = new(Window);
        Camera.BoundsLimit = new(World.Tiles[0, 0].Position, (Vector2f)World.TileCount * TileDrawable.DefaultTilePixelSize);
        
        // first chunks vertices update
        World.UpdateAllChunksVertices(true);
        World.StartUpdateThreads();
        
        Window.Closed += (_, _) => World.EndUpdateThreads();
        Window.MouseWheelScrolled += (_, args) => Camera.MouseScrollDelta = args.Delta;

        Window.View.Center = World.GetCenterPosition();
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


    private static string WorldGenerationStageToString(WorldGenerationStage stage) => stage switch
    {
        WorldGenerationStage.None => "Starting",
        WorldGenerationStage.NoiseGeneration => "Generating noise",
        WorldGenerationStage.WorldInitialization => "Initializing world",
        WorldGenerationStage.WorldTerrainFilling => "Filling terrain",
        WorldGenerationStage.LoadingChunks => "Loading chunks",
        
        _ => "Doing something"
    };
}