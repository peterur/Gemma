using Godot;
using System;

public partial class Dog : CharacterBody3D
{
    [Export]
    public float Speed = 5.0f;

    [Export]
    public float Acceleration = 10.0f;

    [Export]
    public float BoundarySize = 24.0f;

    [Export]
    public float FrontBoundary = -24.0f;

    [Export]
    public float BounceForce = 8.0f;

    [Export]
    public NodePath CameraPath = "../Camera3D";

    private Vector3 _targetVelocity = Vector3.Zero;
    private CameraController _camera;
    private const string LogContext = "Dog";

    public override void _Ready()
    {
        try
        {
            Logger.Debug("Dog initializing...", LogContext);

            _camera = GetNodeOrNull<CameraController>(CameraPath);

            if (_camera == null)
            {
                Logger.Warn($"Camera not found at path '{CameraPath}', controls will not be camera-relative", LogContext);
            }
            else
            {
                Logger.Debug("Camera reference acquired", LogContext);
            }

            Logger.Info("Dog initialized successfully", LogContext);
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
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // Transform input to be relative to camera
        Vector3 direction = CalculateCameraRelativeDirection(inputDir);

        _targetVelocity = direction * Speed;

        Vector3 velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, _targetVelocity.X, Acceleration * (float)delta);
        velocity.Z = Mathf.MoveToward(velocity.Z, _targetVelocity.Z, Acceleration * (float)delta);
        velocity.Y = 0;

        // Boundary check and bounce
        Vector3 pos = GlobalPosition;
        velocity = ApplyBoundaryBounce(ref pos, velocity);

        GlobalPosition = pos;
        Velocity = velocity;
        MoveAndSlide();
    }

    private Vector3 CalculateCameraRelativeDirection(Vector2 inputDir)
    {
        if (_camera != null && inputDir != Vector2.Zero)
        {
            float cameraYaw = _camera.GetYRotation();
            float cos = Mathf.Cos(cameraYaw);
            float sin = Mathf.Sin(cameraYaw);

            float rotatedX = inputDir.X * cos - inputDir.Y * sin;
            float rotatedZ = inputDir.X * sin + inputDir.Y * cos;

            return new Vector3(rotatedX, 0, rotatedZ);
        }

        return new Vector3(inputDir.X, 0, inputDir.Y);
    }

    private Vector3 ApplyBoundaryBounce(ref Vector3 pos, Vector3 velocity)
    {
        bool bounced = false;

        if (pos.X > BoundarySize)
        {
            pos.X = BoundarySize;
            velocity.X = -BounceForce;
            bounced = true;
        }
        else if (pos.X < -BoundarySize)
        {
            pos.X = -BoundarySize;
            velocity.X = BounceForce;
            bounced = true;
        }

        if (pos.Z > BoundarySize)
        {
            pos.Z = BoundarySize;
            velocity.Z = -BounceForce;
            bounced = true;
        }
        else if (pos.Z < FrontBoundary)
        {
            pos.Z = FrontBoundary;
            velocity.Z = BounceForce;
            bounced = true;
        }

        if (bounced)
        {
            Logger.Debug($"Dog bounced at position {pos}", LogContext);
        }

        return velocity;
    }
}
