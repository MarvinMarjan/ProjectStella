using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace Stella.Game;


public class Camera
{
    public GameWindow Window { get; private set; }
    
    // how much the mouse position changed since the last frame update
    public Vector2f WorldMousePositionDelta => _oldWorldMousePosition - WorldMousePosition;
    public Vector2f WorldMousePosition => Window.MapPixelToCoords(MousePosition);
    private Vector2f _oldWorldMousePosition;
    public Vector2i MousePosition => Mouse.GetPosition(Window);

    public float MouseScrollDelta { get; set; }

    public Vector2f DefaultViewSize { get; }
    public Vector2f ViewSizeDiff => new(DefaultViewSize.X / Window.View.Size.X, DefaultViewSize.Y / Window.View.Size.Y);
    
    public float ZoomOutFactor { get; private set; } = 1.3f; 
    public float ZoomInFactor { get; private set; } = 0.7f;
    
    public float MaxZoomOut { get; private set; } = 9000f;
    public float MaxZoomIn { get; private set; } = 420f;
    
    public bool IsAtMaxZoom => Window.View.Size.X >= MaxZoomOut;
    public bool IsAtMinZoom => Window.View.Size.Y <= MaxZoomIn;
    
    public static Vector2f DefaultVisibleAreaOffset => new(400f, 400f);
    public Vector2f VisibleAreaOffset { get; private set; }
    
    public bool IsGrabbingView => Window.HasFocus() && Mouse.IsButtonPressed(Mouse.Button.Right);
    
    
    public Camera(GameWindow window)
    {
        Window = window;

        DefaultViewSize = Window.View.Size;
        VisibleAreaOffset = DefaultVisibleAreaOffset;
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

        VisibleAreaOffset = new(DefaultVisibleAreaOffset.X / ViewSizeDiff.X, DefaultVisibleAreaOffset.Y / ViewSizeDiff.Y);
        
        MouseScrollDelta = 0f;
        
        _oldWorldMousePosition = WorldMousePosition;
    }


    public bool IsRectVisibleToCamera(FloatRect rect)
    {
        Vector2f windowPosition = Window.View.Center - Window.View.Size / 2;
        windowPosition.X -= VisibleAreaOffset.X;
        windowPosition.Y -= VisibleAreaOffset.Y;

        Vector2f windowViewSize = Window.View.Size;
        windowViewSize.X += VisibleAreaOffset.X * 2;
        windowViewSize.Y += VisibleAreaOffset.Y * 2;
        
        return rect.Intersects(new(windowPosition, windowViewSize));
    }
}