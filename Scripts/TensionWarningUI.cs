using Godot;
using System;

public partial class TensionWarningUI : Control
{
    [Export]
    public NodePath LeashPath = "../../Leash";

    private Leash _leash;
    private Label _warningLabel;
    private ColorRect _warningBackground;
    private float _flashTimer = 0f;
    private bool _flashState = false;
    private const string LogContext = "TensionWarningUI";

    public override void _Ready()
    {
        try
        {
            Logger.Debug("TensionWarningUI initializing...", LogContext);

            _leash = GetNodeOrNull<Leash>(LeashPath);

            // Create warning background
            _warningBackground = new ColorRect();
            _warningBackground.Color = new Color(1f, 0f, 0f, 0.3f);
            _warningBackground.SetAnchorsPreset(LayoutPreset.TopWide);
            _warningBackground.CustomMinimumSize = new Vector2(0, 60);
            _warningBackground.Visible = false;
            AddChild(_warningBackground);

            // Create warning label
            _warningLabel = new Label();
            _warningLabel.Text = "LEASH TENSION!";
            _warningLabel.HorizontalAlignment = HorizontalAlignment.Center;
            _warningLabel.VerticalAlignment = VerticalAlignment.Center;
            _warningLabel.SetAnchorsPreset(LayoutPreset.TopWide);
            _warningLabel.CustomMinimumSize = new Vector2(0, 60);
            _warningLabel.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f));
            _warningLabel.AddThemeFontSizeOverride("font_size", 32);
            _warningLabel.Visible = false;
            AddChild(_warningLabel);

            // Connect to leash signal
            if (_leash != null)
            {
                _leash.LeashTensionChanged += OnLeashTensionChanged;
                Logger.Debug("Connected to leash tension signal", LogContext);
            }
            else
            {
                Logger.Warn($"Leash not found at path '{LeashPath}', tension warning will not work", LogContext);
            }

            Logger.Info("TensionWarningUI initialized successfully", LogContext);
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
            ProcessFlash(delta);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, LogContext);
        }
    }

    private void ProcessFlash(double delta)
    {
        if (_warningLabel == null || !_warningLabel.Visible)
            return;

        _flashTimer += (float)delta;
        if (_flashTimer >= 0.15f)
        {
            _flashTimer = 0f;
            _flashState = !_flashState;
            _warningBackground.Color = _flashState
                ? new Color(1f, 0f, 0f, 0.5f)
                : new Color(1f, 0.3f, 0f, 0.3f);
        }
    }

    private void OnLeashTensionChanged(bool isTense, float tensionAmount)
    {
        _warningLabel.Visible = isTense;
        _warningBackground.Visible = isTense;
        _flashTimer = 0f;
    }

    public override void _ExitTree()
    {
        if (_leash != null)
        {
            _leash.LeashTensionChanged -= OnLeashTensionChanged;
        }
    }
}
