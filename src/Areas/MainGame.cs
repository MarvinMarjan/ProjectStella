using SFML.System;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;

using Stella.Game;
using Stella.Game.Tiles;
using Stella.Game.World;


namespace Stella.Areas;


// TODO: add ability to change the tiles in the world.
// TODO: add world saving. save seed and tiles changed by the player; use binary files to store it
// TODO: add interface (GUI) to create, select and remove worlds.
// TODO: add animations to the tiles
// TODO: add lighting (day cycle maybe?)


public class MainGame : Area
{
    public ProgressBarPopup WorldGenerationProgressPopup { get; }
    public TextElement WorldGenerationStageText { get; }
    
    public TileWorld? World => WorldGenerator.TileWorld;

    public WorldGenerator WorldGenerator { get; }
    public bool WorldGenerated { get; private set; }
    
    public Camera? Camera { get; private set; }
    
    
    public MainGame(MainWindow window) : base(window)
    {
        WorldGenerator = new(App.MainView, new(64 * 20, 64 * 20));
        WorldGenerator.StartWorldGeneration();
        
        WorldGenerationProgressPopup = new("World Generation Progress");
        WorldGenerationProgressPopup.Show();

        WorldGenerationProgressPopup.UpdateEvent += (_, _) => OnWorldGenerationProgressPopupUpdate();
        WorldGenerationProgressPopup.CloseEvent += (_, _) => OnWorldGenerated();
        WorldGenerationProgressPopup.CloseOnComplete = false;
        
        WorldGenerationStageText = new(WorldGenerationProgressPopup, new(), 30, "Starting")
        {
            Alignment = { Value = Alignments.Center }
        };
    }

    
    public sealed override void Update()
    {
        if (WorldGenerated)
            World?.Update();

        if (WorldGenerator.Stage == WorldGenerationStage.Finished)
            WorldGenerationProgressPopup.Close();
        
        Camera?.Update();
    }
    
    private void OnWorldGenerationProgressPopupUpdate()
    {
        WorldGenerationStageText.Text.Set(WorldGenerationStageToString(WorldGenerator.Stage));
            
        if (World is not null)
            WorldGenerationProgressPopup.ProgressBar.Progress.Set(CalculateWorldGenerationProgress(World));
    }

    private void OnWorldGenerated()
    {
        if (World is null)
            return;
        
        WorldGenerated = true;
        
        Camera = new(App.Window, App.MainView);
        Camera.BoundsLimit = new(World.Tiles[0, 0].Position, (Vector2f)World.TileCount * TileDrawable.DefaultTilePixelSize);
        
        World.StartUpdateThreads();
    }


    public sealed override void Draw(RenderTarget target)
    {
        if (WorldGenerated)
            World?.Draw(Window);
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