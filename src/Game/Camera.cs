using SFML.System;
using SFML.Window;


namespace Stella.Game;


public class Camera(GameWindow window)
{
    public GameWindow Window { get; private set; } = window;
    
    // how much the mouse position changed since the last frame update
    public Vector2f WorldMouseDelta => _oldWorldMousePosition - WorldMousePosition;
    public Vector2i MousePosition => Mouse.GetPosition(Window);
    public Vector2f WorldMousePosition => Window.MapPixelToCoords(MousePosition);
    private Vector2f _oldWorldMousePosition = new();
    
    public float MouseScrollDelta { get; set; }

    public const float ZoomOutFactor = 1.3f; 
    public const float ZoomInFactor = 0.7f;

    public const float MaxZoomOut = 6000f;
    public const float MaxZoomIn = 420f;

    public bool IsGrabbingView => Window.HasFocus() && Mouse.IsButtonPressed(Mouse.Button.Right);


    public void Update()
    {
        if (IsGrabbingView)
            Window.CurrentView.Move(WorldMouseDelta);

        bool atMinSize = Window.CurrentView.Size.X <= MaxZoomIn;
        bool atMaxSize = Window.CurrentView.Size.X >= MaxZoomOut;
        
        if (MouseScrollDelta != 0f && (MouseScrollDelta > 0 ? !atMinSize : !atMaxSize))
        {
            float zoom = MouseScrollDelta > 0 ? ZoomInFactor : ZoomOutFactor;
            Window.CurrentView.Zoom(zoom);
        }
        
        MouseScrollDelta = 0f;
        
        _oldWorldMousePosition = WorldMousePosition;
    }
}