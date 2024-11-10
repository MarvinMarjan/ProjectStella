using SFML.System;
using SFML.Window;
using SFML.Graphics;


namespace Stella.Game;


public readonly struct BoundLimitCollisions
{
    public required bool AtLeft { get; init; }
    public required bool AtRight { get; init; }
    public required bool AtTop { get; init; }
    public required bool AtBottom { get; init; }
    
    
    public bool HasAnyCollision()
        => AtLeft || AtRight || AtTop || AtBottom;
    
    public bool LeftRightCollision() => AtLeft && AtRight;
    public bool TopBottomCollision() => AtTop && AtBottom;
}


public class Camera
{
    public MainWindow Window { get; private set; }
    
    // how much the mouse position changed since the last frame update
    public Vector2f WorldMousePositionDelta => _oldWorldMousePosition - Window.WorldMousePosition;
    private Vector2f _oldWorldMousePosition;

    public float MouseScrollDelta { get; set; }

    public Vector2f DefaultViewSize { get; }

    public FloatRect Bounds => Window.View.ViewToRect();
    public FloatRect BoundsLimit { get; set; }
    
    public float ZoomOutFactor { get; private set; } = 1.2f; 
    public float ZoomInFactor { get; private set; } = 0.8f;
    
    public float MaxZoomOut { get; private set; } = 8000f;
    public float MaxZoomIn { get; private set; } = 100f;
    
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
            ProcessViewGrabMovement();
        
        if (MouseScrollDelta != 0f && (MouseScrollDelta > 0 ? !IsAtMinZoom : !IsAtMaxZoom))
        {
            float zoom = MouseScrollDelta > 0 ? ZoomInFactor : ZoomOutFactor;
            Window.View.Zoom(zoom);
        }

        CheckBoundsLimits();
        
        MouseScrollDelta = 0f;
        
        _oldWorldMousePosition = Window.WorldMousePosition;
    }

    private void ProcessViewGrabMovement()
    {
        BoundLimitCollisions collisions = GetBoundLimitCollisions();
        Vector2f delta = WorldMousePositionDelta;

        if (collisions.LeftRightCollision())
            delta.X = 0;
            
        if (collisions.TopBottomCollision())
            delta.Y = 0;
            
        Window.View.Move(delta);
    }
    
    private void CheckBoundsLimits()
    {
        BoundLimitCollisions collisions = GetBoundLimitCollisions();

        bool horizontalLocked = false;
        bool verticalLocked = false;

        if (collisions.LeftRightCollision())
        {
            Window.View.Center = Window.View.Center with { X = BoundsLimit.Position.X + BoundsLimit.Size.X / 2f };
            horizontalLocked = true;
        }

        if (collisions.TopBottomCollision())
        {
            Window.View.Center = Window.View.Center with { Y = BoundsLimit.Position.Y + BoundsLimit.Size.Y / 2f };
            verticalLocked = true;
        }
        
        if (collisions.AtLeft && !horizontalLocked)
            Window.View.MoveToPosition(new(BoundsLimit.Position.X, Bounds.Position.Y));
        
        else if (collisions.AtRight && !horizontalLocked)
            Window.View.MoveToPosition(new(BoundsLimit.Position.X + BoundsLimit.Width - Bounds.Width, Bounds.Position.Y));
        
        if (collisions.AtTop && !verticalLocked)
            Window.View.MoveToPosition(new(Bounds.Position.X, BoundsLimit.Position.Y));
        
        else if (collisions.AtBottom && !verticalLocked)
            Window.View.MoveToPosition(new(Bounds.Position.X, BoundsLimit.Position.Y + BoundsLimit.Height - Bounds.Height));
        
    }


    private BoundLimitCollisions GetBoundLimitCollisions() => new()
    {
        AtTop = Bounds.Position.Y < BoundsLimit.Position.Y,
        AtLeft = Bounds.Position.X < BoundsLimit.Position.X,
        AtRight = Bounds.Position.X + Bounds.Width > BoundsLimit.Position.X + BoundsLimit.Width,
        AtBottom = Bounds.Position.Y + Bounds.Height > BoundsLimit.Position.Y + BoundsLimit.Height
    };


    public bool IsRectVisibleToCamera(FloatRect rect)
        => Window.View.IsRectVisibleToView(rect);
}