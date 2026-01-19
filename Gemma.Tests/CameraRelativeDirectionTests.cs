using Xunit;

namespace Gemma.Tests;

/// <summary>
/// Tests for camera-relative direction calculations used in Dog movement
/// </summary>
public class CameraRelativeDirectionTests
{
    private const float Tolerance = 0.001f;

    [Fact]
    public void NoRotation_InputUnchanged()
    {
        // When camera has no rotation, input should remain the same
        float cameraYaw = 0f;
        var input = (X: 1f, Y: 0f);  // Moving right

        var result = CalculateCameraRelativeDirection(input.X, input.Y, cameraYaw);

        Assert.Equal(input.X, result.X, Tolerance);
        Assert.Equal(input.Y, result.Z, Tolerance);
    }

    [Fact]
    public void Rotate90Degrees_InputRotated()
    {
        // When camera is rotated 90 degrees (PI/2), right becomes forward
        float cameraYaw = MathF.PI / 2f;
        var input = (X: 1f, Y: 0f);  // Moving right

        var result = CalculateCameraRelativeDirection(input.X, input.Y, cameraYaw);

        // After 90 degree rotation, X becomes -Z and Y becomes X
        Assert.True(MathF.Abs(result.X) < Tolerance);  // X should be ~0
        Assert.True(MathF.Abs(result.Z - 1f) < Tolerance);  // Z should be ~1
    }

    [Fact]
    public void Rotate180Degrees_InputReversed()
    {
        // When camera is rotated 180 degrees (PI), right becomes left
        float cameraYaw = MathF.PI;
        var input = (X: 1f, Y: 0f);  // Moving right

        var result = CalculateCameraRelativeDirection(input.X, input.Y, cameraYaw);

        // After 180 degree rotation, direction is reversed
        Assert.True(MathF.Abs(result.X + 1f) < Tolerance);  // X should be ~-1
        Assert.True(MathF.Abs(result.Z) < Tolerance);  // Z should be ~0
    }

    [Fact]
    public void ZeroInput_ReturnsZero()
    {
        float cameraYaw = MathF.PI / 4f;  // Any rotation
        var input = (X: 0f, Y: 0f);

        var result = CalculateCameraRelativeDirection(input.X, input.Y, cameraYaw);

        Assert.Equal(0f, result.X, Tolerance);
        Assert.Equal(0f, result.Z, Tolerance);
    }

    [Theory]
    [InlineData(0f, 1f, 0f, 1f)]        // Forward
    [InlineData(1f, 0f, 1f, 0f)]        // Right
    [InlineData(0f, -1f, 0f, -1f)]      // Backward
    [InlineData(-1f, 0f, -1f, 0f)]      // Left
    public void NoRotation_AllDirections(float inputX, float inputY, float expectedX, float expectedZ)
    {
        float cameraYaw = 0f;

        var result = CalculateCameraRelativeDirection(inputX, inputY, cameraYaw);

        Assert.Equal(expectedX, result.X, Tolerance);
        Assert.Equal(expectedZ, result.Z, Tolerance);
    }

    [Fact]
    public void DiagonalInput_MaintainsMagnitude()
    {
        float cameraYaw = MathF.PI / 4f;  // 45 degrees
        var input = (X: 0.707f, Y: 0.707f);  // Diagonal (approximately normalized)

        var result = CalculateCameraRelativeDirection(input.X, input.Y, cameraYaw);

        // Magnitude should be preserved (approximately 1)
        float magnitude = MathF.Sqrt(result.X * result.X + result.Z * result.Z);
        Assert.True(MathF.Abs(magnitude - 1f) < 0.01f);
    }

    private (float X, float Z) CalculateCameraRelativeDirection(float inputX, float inputY, float cameraYaw)
    {
        float cos = MathF.Cos(cameraYaw);
        float sin = MathF.Sin(cameraYaw);

        float rotatedX = inputX * cos - inputY * sin;
        float rotatedZ = inputX * sin + inputY * cos;

        return (rotatedX, rotatedZ);
    }
}
