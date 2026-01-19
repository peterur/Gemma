using Godot;
using System;

public partial class CameraController : Camera3D
{
    [Export]
    public float MoveSpeed = 10.0f;

    [Export]
    public float RotateSpeed = 2.0f;

    [Export]
    public float VerticalSpeed = 5.0f;

    // Home position - behind the shoulder view of the whole map
    private readonly Vector3 _homePosition = new Vector3(0, 12, 18);
    private readonly Vector3 _homeRotation = new Vector3(Mathf.DegToRad(-35f), 0, 0);
    private const string LogContext = "Camera";

    public override void _Ready()
    {
        try
        {
            Logger.Debug("Camera initializing...", LogContext);
            GoHome();
            Logger.Info("Camera initialized successfully", LogContext);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    public override void _Process(double delta)
    {
        try
        {
            ProcessCamera(delta);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    private void ProcessCamera(double delta)
    {
        float dt = (float)delta;

        // Home key
        if (Input.IsActionJustPressed("camera_home"))
        {
            GoHome();
            return;
        }

        // Get camera movement input (WASD)
        Vector3 moveDir = Vector3.Zero;

        if (Input.IsActionPressed("camera_forward"))
            moveDir -= Transform.Basis.Z;
        if (Input.IsActionPressed("camera_back"))
            moveDir += Transform.Basis.Z;
        if (Input.IsActionPressed("camera_left"))
            moveDir -= Transform.Basis.X;
        if (Input.IsActionPressed("camera_right"))
            moveDir += Transform.Basis.X;

        // Vertical movement (R/F)
        if (Input.IsActionPressed("camera_up"))
            moveDir += Vector3.Up;
        if (Input.IsActionPressed("camera_down"))
            moveDir += Vector3.Down;

        // Apply movement
        if (moveDir != Vector3.Zero)
        {
            GlobalPosition += moveDir.Normalized() * MoveSpeed * dt;
        }

        // Rotation (Q/E + right stick)
        float rotateInput = 0f;
        if (Input.IsActionPressed("camera_rotate_left"))
            rotateInput += 1f;
        if (Input.IsActionPressed("camera_rotate_right"))
            rotateInput -= 1f;

        if (rotateInput != 0f)
        {
            RotateY(rotateInput * RotateSpeed * dt);
        }

        // Right stick vertical look (pitch)
        float lookVertical = Input.GetActionStrength("camera_down") - Input.GetActionStrength("camera_up");
        if (Mathf.Abs(lookVertical) > 0.1f)
        {
            float newPitch = Rotation.X + lookVertical * RotateSpeed * dt;
            newPitch = Mathf.Clamp(newPitch, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            Rotation = new Vector3(newPitch, Rotation.Y, Rotation.Z);
        }
    }

    public void GoHome()
    {
        GlobalPosition = _homePosition;
        Rotation = _homeRotation;
    }

    // Get the camera's Y rotation for relative controls
    public float GetYRotation()
    {
        return Rotation.Y;
    }
}
