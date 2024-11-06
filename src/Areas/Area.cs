using SFML.Graphics;


namespace Stella.Areas;


public abstract class Area(MainWindow window) : IUpdateable, IDrawable
{
    public MainWindow Window { get; } = window;
    
    
    public abstract void Update();
    public abstract void Draw(RenderTarget target);
}