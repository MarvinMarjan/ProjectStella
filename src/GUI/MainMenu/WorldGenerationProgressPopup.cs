using SFML.System;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;

using Stella.Game.World;


namespace Stella.GUI.MainMenu;


public class WorldGenerationProgressPopup : ProgressBarPopup
{
    public TextElement GenerationStageText { get; }

    public TileWorld? World => WorldGenerator.TileWorld;
    public WorldGenerator WorldGenerator { get; }
    
    
    public WorldGenerationProgressPopup(WorldGenerator generator) : base("World Generation Progress")
    {
        WorldGenerator = generator;
        
        Alignment.Value = Alignments.TopLeft;

        Styles = WindowElementStyles.None;
        
        Color.Value = new(0, 0, 0, 220);
        
        GenerationStageText = new(this, new(), 30, "Starting")
        {
            Alignment = { Value = Alignments.Center }
        };
        
        ProgressBar.AlignmentMargin.Set(new(0, -100));
        
        CloseOnComplete = false;
    }


    protected sealed override void Setup()
    {
        WorldGenerator.StartWorldGeneration();
        
        base.Setup();
    }


    public sealed override void Update()
    {
        Size.Value = (Vector2f)App.Window.Size;
        
        GenerationStageText.Text.Set(WorldGenerator.WorldGenerationStageToString(WorldGenerator.Stage));
        
        if (World is not null)
            ProgressBar.Progress.Set(WorldGenerator.CalculateWorldGenerationProgress(World));
        
        if (WorldGenerator.Stage == WorldGenerationStage.Finished)
            Close();
        
        base.Update();
    }
}