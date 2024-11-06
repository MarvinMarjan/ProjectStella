using SFML.Graphics;


namespace Stella;


public interface IUpdateable
{
    void Update();
}


public interface IDrawable
{
    void Draw(RenderTarget target);
}