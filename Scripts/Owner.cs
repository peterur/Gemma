using Godot;

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

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = MoveDirection.Normalized() * Speed;
        velocity.Y = 0;

        // Boundary check and bounce
        Vector3 pos = GlobalPosition;
        if (pos.X > BoundarySize)
        {
            pos.X = BoundarySize;
            MoveDirection.X = -Mathf.Abs(MoveDirection.X);
        }
        else if (pos.X < -BoundarySize)
        {
            pos.X = -BoundarySize;
            MoveDirection.X = Mathf.Abs(MoveDirection.X);
        }

        if (pos.Z > BoundarySize)
        {
            pos.Z = BoundarySize;
            MoveDirection.Z = -Mathf.Abs(MoveDirection.Z);
        }
        else if (pos.Z < FrontBoundary)
        {
            pos.Z = FrontBoundary;
            MoveDirection.Z = Mathf.Abs(MoveDirection.Z);
        }

        GlobalPosition = pos;
        Velocity = MoveDirection.Normalized() * Speed;
        MoveAndSlide();
    }
}
