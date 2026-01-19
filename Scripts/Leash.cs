using Godot;
using System;

public partial class Leash : Node3D
{
    [Signal]
    public delegate void LeashTensionChangedEventHandler(bool isTense, float tensionAmount);

    [Export]
    public float MaxLength = 3.0f;

    [Export]
    public float PullStrength = 15.0f;

    [Export]
    public float TensionThreshold = 0.7f;

    [Export]
    public NodePath OwnerPath = "../Owner";

    [Export]
    public NodePath DogPath = "../Dog";

    private CharacterBody3D _owner;
    private CharacterBody3D _dog;
    private ImmediateMesh _leashMesh;
    private MeshInstance3D _leashVisual;
    private StandardMaterial3D _leashMaterial;
    private bool _wasTense = false;
    private const string LogContext = "Leash";

    public bool IsTense { get; private set; } = false;
    public float TensionAmount { get; private set; } = 0f;

    public override void _Ready()
    {
        try
        {
            Logger.Debug("Leash initializing...", LogContext);

            _owner = GetNodeOrNull<CharacterBody3D>(OwnerPath);
            _dog = GetNodeOrNull<CharacterBody3D>(DogPath);

            if (_owner == null)
            {
                Logger.Error($"Owner not found at path '{OwnerPath}'", LogContext);
            }

            if (_dog == null)
            {
                Logger.Error($"Dog not found at path '{DogPath}'", LogContext);
            }

            _leashMesh = new ImmediateMesh();
            _leashMaterial = new StandardMaterial3D();
            _leashMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            _leashMaterial.AlbedoColor = new Color(0.4f, 0.2f, 0.1f);
            _leashMaterial.CullMode = BaseMaterial3D.CullModeEnum.Disabled;

            _leashVisual = new MeshInstance3D();
            _leashVisual.Mesh = _leashMesh;
            _leashVisual.MaterialOverride = _leashMaterial;
            AddChild(_leashVisual);

            Logger.Info("Leash initialized successfully", LogContext);
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
            if (_owner == null || _dog == null)
                return;

            ProcessLeash(delta);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    private void ProcessLeash(double delta)
    {
        if (_owner == null || _dog == null)
            return;

        Vector3 ownerPos = _owner.GlobalPosition;
        Vector3 dogPos = _dog.GlobalPosition;

        Vector3 toOwner = ownerPos - dogPos;
        float distance = toOwner.Length();

        TensionAmount = Mathf.Clamp((distance / MaxLength), 0f, 1f);
        IsTense = TensionAmount > TensionThreshold;

        if (distance > MaxLength)
        {
            Vector3 pullDir = toOwner.Normalized();
            float overStretch = distance - MaxLength;

            Vector3 dogVel = _dog.Velocity;
            dogVel += pullDir * PullStrength * overStretch * (float)delta;
            _dog.Velocity = dogVel;
        }

        // Update leash color based on tension
        if (IsTense)
        {
            float redAmount = Mathf.Lerp(0.4f, 1.0f, (TensionAmount - TensionThreshold) / (1f - TensionThreshold));
            _leashMaterial.AlbedoColor = new Color(redAmount, 0.2f, 0.1f);
        }
        else
        {
            _leashMaterial.AlbedoColor = new Color(0.4f, 0.2f, 0.1f);
        }

        // Rumble feedback when tense
        if (IsTense)
        {
            float rumbleStrength = (TensionAmount - TensionThreshold) / (1f - TensionThreshold);
            Input.StartJoyVibration(0, rumbleStrength * 0.3f, rumbleStrength * 0.5f, 0.1f);
        }
        else if (_wasTense)
        {
            Input.StopJoyVibration(0);
        }

        // Emit signal when tension state changes
        if (IsTense != _wasTense)
        {
            EmitSignal(SignalName.LeashTensionChanged, IsTense, TensionAmount);
        }
        _wasTense = IsTense;

        DrawLeash(ownerPos, dogPos);
    }

    private void DrawLeash(Vector3 from, Vector3 to)
    {
        _leashMesh.ClearSurfaces();
        _leashMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

        Vector3 ownerAttach = from + new Vector3(0, -0.5f, 0);
        Vector3 dogAttach = to + new Vector3(0, 0.2f, 0);

        _leashMesh.SurfaceAddVertex(ownerAttach);
        _leashMesh.SurfaceAddVertex(dogAttach);

        _leashMesh.SurfaceEnd();
    }
}
