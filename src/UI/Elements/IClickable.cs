using System;

using SFML.System;


namespace Stella.UI.Elements;


public interface IClickable
{
    event EventHandler? MouseEnterEvent;
    event EventHandler? MouseLeaveEvent;
    event EventHandler? MouseDownEvent;
    event EventHandler? MouseUpEvent;
    
    bool IsPointOver(Vector2f point);
}