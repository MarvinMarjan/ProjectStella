using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements.Primitives.Shapes;

using Stella.GUI;
using Stella.Game.World;


namespace Stella.Areas;


public class WorldMenu : Area
{
    public TileWorld BackgroundWorld { get; }
    
    public RectangleElement MainContainer { get; }
    public Button NewWorldButton { get; }
    
    
    public WorldMenu(MainWindow window, TileWorld backgroundWorld) : base(window)
    {
        BackgroundWorld = backgroundWorld;

        MainContainer = new(null, new(), new(700, 800), 7f)
        {
            Alignment = { Value = Alignments.Center },
            
            BorderSize = { Value = 2f },
            BorderColor = { Value = new(0, 0, 0, 230) },
            Color = { Value = new(0, 0, 0, 200) }
        };

        NewWorldButton = new(MainContainer, new(), new(150, 45), "New World")
        {
            Alignment = { Value = Alignments.BottomRight },
            AlignmentMargin = { Value = new(-15, -10) }
        };
    }


    public sealed override void Deinitialize()
    {
        base.Deinitialize();
        
        App.RemoveElement(MainContainer);
    }


    public sealed override void Update()
    {
        BackgroundWorld.Update();
    }


    public sealed override void Draw(RenderTarget target)
    {
        BackgroundWorld.Draw(target);
    }
}