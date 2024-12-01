using System;

using SFML.Graphics;

using Latte.Core;


namespace Stella.Areas;


public abstract class Area(MainWindow window) : IUpdateable, IDrawable
{
    public MainWindow Window { get; } = window;
    
    public event EventHandler? InitializeEvent;
    public event EventHandler? DeinitializeEvent;


    public virtual void Initialize() => InitializeEvent?.Invoke(this, EventArgs.Empty);
    public virtual void Deinitialize() => DeinitializeEvent?.Invoke(this, EventArgs.Empty);
    
    
    public abstract void Update();
    public abstract void Draw(RenderTarget target);
}