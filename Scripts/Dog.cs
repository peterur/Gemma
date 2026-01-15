using Godot;

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

    public override void _Ready()
    {
        _camera = GetNodeOrNull<CameraController>(CameraPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // Transform input to be relative to camera
        Vector3 direction;
        if (_camera != null && inputDir != Vector2.Zero)
        {
            float cameraYaw = _camera.GetYRotation();
            float cos = Mathf.Cos(cameraYaw);
            float sin = Mathf.Sin(cameraYaw);

            // Rotate input by camera's Y rotation
            float rotatedX = inputDir.X * cos - inputDir.Y * sin;
            float rotatedZ = inputDir.X * sin + inputDir.Y * cos;

            direction = new Vector3(rotatedX, 0, rotatedZ);
        }
        else
        {
            direction = new Vector3(inputDir.X, 0, inputDir.Y);
        }

        _targetVelocity = direction * Speed;

        Vector3 velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, _targetVelocity.X, Acceleration * (float)delta);
        velocity.Z = Mathf.MoveToward(velocity.Z, _targetVelocity.Z, Acceleration * (float)delta);
        velocity.Y = 0;

        // Boundary check and bounce
        Vector3 pos = GlobalPosition;
        if (pos.X > BoundarySize)
        {
            pos.X = BoundarySize;
            velocity.X = -BounceForce;
        }
        else if (pos.X < -BoundarySize)
        {
            pos.X = -BoundarySize;
            velocity.X = BounceForce;
        }

        if (pos.Z > BoundarySize)
        {
            pos.Z = BoundarySize;
            velocity.Z = -BounceForce;
        }
        else if (pos.Z < FrontBoundary)
        {
            pos.Z = FrontBoundary;
            velocity.Z = BounceForce;
        }

        GlobalPosition = pos;
        Velocity = velocity;
        MoveAndSlide();
    }
}
