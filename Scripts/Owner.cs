using Godot;
using System;

public partial class Owner : CharacterBody3D
{
    [Export]
    public float Speed = 3.0f;

    [Export]
    public Vector3 MoveDirection = new Vector3(0, 0, -1);

    [Export]
    public float BoundarySize = 24.0f;

    [Export]
    public float FrontBoundary = -24.0f;

    [Export]
    public float BounceForce = 5.0f;

    private const string LogContext = "Owner";

    public override void _Ready()
    {
        try
        {
            Logger.Debug("Owner initialized", LogContext);

            if (MoveDirection == Vector3.Zero)
            {
                Logger.Warn("MoveDirection is zero, defaulting to forward", LogContext);
                MoveDirection = new Vector3(0, 0, -1);
            }
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        try
        {
            ProcessMovement(delta);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    private void ProcessMovement(double delta)
    {
        Vector3 velocity = MoveDirection.Normalized() * Speed;
        velocity.Y = 0;

        Vector3 pos = GlobalPosition;
        bool bounced = false;

        // Boundary check and bounce - X axis
        if (pos.X > BoundarySize)
        {
            pos.X = BoundarySize;
            MoveDirection.X = -Mathf.Abs(MoveDirection.X);
            bounced = true;
        }
        else if (pos.X < -BoundarySize)
        {
            pos.X = -BoundarySize;
            MoveDirection.X = Mathf.Abs(MoveDirection.X);
            bounced = true;
        }

        // Boundary check and bounce - Z axis
        if (pos.Z > BoundarySize)
        {
            pos.Z = BoundarySize;
            MoveDirection.Z = -Mathf.Abs(MoveDirection.Z);
            bounced = true;
        }
        else if (pos.Z < FrontBoundary)
        {
            pos.Z = FrontBoundary;
            MoveDirection.Z = Mathf.Abs(MoveDirection.Z);
            bounced = true;
        }

        if (bounced)
        {
            Logger.Debug($"Owner bounced at position {pos}", LogContext);
        }

        GlobalPosition = pos;
        Velocity = MoveDirection.Normalized() * Speed;
        MoveAndSlide();
    }
}
