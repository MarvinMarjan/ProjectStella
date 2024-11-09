using SFML.System;
using SFML.Window;
using SFML.Graphics;


namespace Stella.Game;


public class Camera
{
    public MainWindow Window { get; private set; }
    
    // how much the mouse position changed since the last frame update
    public Vector2f WorldMousePositionDelta => _oldWorldMousePosition - Window.WorldMousePosition;
    private Vector2f _oldWorldMousePosition;

    public float MouseScrollDelta { get; set; }

    public Vector2f DefaultViewSize { get; }
    
    public float ZoomOutFactor { get; private set; } = 1.3f; 
    public float ZoomInFactor { get; private set; } = 0.7f;
    
    public float MaxZoomOut { get; private set; } = 7000f;
    public float MaxZoomIn { get; private set; } = 200f;
    
    public bool IsAtMaxZoom => Window.View.Size.X >= MaxZoomOut;
    public bool IsAtMinZoom => Window.View.Size.Y <= MaxZoomIn;
    
    public bool IsGrabbingView => Window.HasFocus() && Mouse.IsButtonPressed(Mouse.Button.Right);
    
    
    public Camera(MainWindow window)
    {
        Window = window;
        
        DefaultViewSize = Window.View.Size;
    }
    

    public void Update()
    {
        if (IsGrabbingView)
            Window.View.Move(WorldMousePositionDelta);
        
        if (MouseScrollDelta != 0f && (MouseScrollDelta > 0 ? !IsAtMinZoom : !IsAtMaxZoom))
        {
            float zoom = MouseScrollDelta > 0 ? ZoomInFactor : ZoomOutFactor;
            Window.View.Zoom(zoom);
        }
        
        MouseScrollDelta = 0f;
        
        _oldWorldMousePosition = Window.WorldMousePosition;
    }


    public bool IsRectVisibleToCamera(FloatRect rect)
        => Window.View.IsRectVisibleToView(rect);
}