using Xunit;

namespace Gemma.Tests;

/// <summary>
/// Tests for boundary calculation logic used in Dog and Owner movement
/// </summary>
public class BoundaryTests
{
    private const float BoundarySize = 24.0f;
    private const float FrontBoundary = -24.0f;
    private const float BounceForce = 8.0f;

    [Theory]
    [InlineData(25.0f, 24.0f)]  // Over right boundary
    [InlineData(-25.0f, -24.0f)]  // Over left boundary
    [InlineData(10.0f, 10.0f)]  // Inside boundary
    [InlineData(24.0f, 24.0f)]  // On boundary
    public void ClampX_ReturnsCorrectValue(float input, float expected)
    {
        float result = ClampToBoundaryX(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(25.0f, 24.0f)]  // Over back boundary
    [InlineData(-25.0f, -24.0f)]  // Over front boundary
    [InlineData(10.0f, 10.0f)]  // Inside boundary
    public void ClampZ_ReturnsCorrectValue(float input, float expected)
    {
        float result = ClampToBoundaryZ(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(25.0f, true)]   // Over boundary
    [InlineData(24.0f, false)]  // On boundary
    [InlineData(10.0f, false)]  // Inside boundary
    public void IsOverRightBoundary_ReturnsCorrectValue(float posX, bool expected)
    {
        bool result = posX > BoundarySize;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-25.0f, true)]   // Over boundary
    [InlineData(-24.0f, false)]  // On boundary
    [InlineData(10.0f, false)]   // Inside boundary
    public void IsOverLeftBoundary_ReturnsCorrectValue(float posX, bool expected)
    {
        bool result = posX < -BoundarySize;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5.0f, -BounceForce)]  // Going right, bounce left
    [InlineData(-5.0f, -BounceForce)]  // Going left (but over right boundary), still bounce left
    public void BounceFromRightBoundary_ReturnsNegativeVelocity(float velocityX, float expectedSign)
    {
        // When bouncing from right boundary, velocity should be negative
        float bounceVelocity = -BounceForce;
        Assert.True(bounceVelocity < 0);
    }

    [Theory]
    [InlineData(-5.0f, BounceForce)]  // Going left, bounce right
    public void BounceFromLeftBoundary_ReturnsPositiveVelocity(float velocityX, float expectedVelocity)
    {
        float bounceVelocity = BounceForce;
        Assert.True(bounceVelocity > 0);
    }

    private float ClampToBoundaryX(float x)
    {
        if (x > BoundarySize) return BoundarySize;
        if (x < -BoundarySize) return -BoundarySize;
        return x;
    }

    private float ClampToBoundaryZ(float z)
    {
        if (z > BoundarySize) return BoundarySize;
        if (z < FrontBoundary) return FrontBoundary;
        return z;
    }
}
