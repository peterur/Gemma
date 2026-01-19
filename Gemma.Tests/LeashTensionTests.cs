using Xunit;

namespace Gemma.Tests;

/// <summary>
/// Tests for leash tension calculation logic
/// </summary>
public class LeashTensionTests
{
    private const float MaxLength = 3.0f;
    private const float TensionThreshold = 0.7f;

    [Theory]
    [InlineData(0.0f, 0.0f)]     // No distance, no tension
    [InlineData(1.5f, 0.5f)]     // Half max length
    [InlineData(3.0f, 1.0f)]     // At max length
    [InlineData(4.5f, 1.0f)]     // Over max length (clamped to 1)
    public void CalculateTensionAmount_ReturnsCorrectValue(float distance, float expectedTension)
    {
        float tension = CalculateTensionAmount(distance);
        Assert.Equal(expectedTension, tension, 2);  // 2 decimal precision
    }

    [Theory]
    [InlineData(0.0f, false)]    // No tension
    [InlineData(1.5f, false)]    // Below threshold (0.5 < 0.7)
    [InlineData(2.1f, false)]    // Just at threshold (0.7 = 0.7)
    [InlineData(2.5f, true)]     // Above threshold
    [InlineData(3.0f, true)]     // At max
    [InlineData(4.0f, true)]     // Over max
    public void IsTense_ReturnsCorrectValue(float distance, bool expectedTense)
    {
        float tension = CalculateTensionAmount(distance);
        bool isTense = tension > TensionThreshold;
        Assert.Equal(expectedTense, isTense);
    }

    [Theory]
    [InlineData(3.5f, 0.5f)]     // 0.5 over max
    [InlineData(4.0f, 1.0f)]     // 1.0 over max
    [InlineData(3.0f, 0.0f)]     // At max (no overstretch)
    [InlineData(2.0f, 0.0f)]     // Under max (no overstretch)
    public void CalculateOverstretch_ReturnsCorrectValue(float distance, float expectedOverstretch)
    {
        float overstretch = CalculateOverstretch(distance);
        Assert.Equal(expectedOverstretch, overstretch, 2);
    }

    [Fact]
    public void TensionThreshold_IsReasonableValue()
    {
        // Threshold should be between 0 and 1
        Assert.True(TensionThreshold > 0.0f);
        Assert.True(TensionThreshold < 1.0f);
    }

    [Fact]
    public void MaxLength_IsPositive()
    {
        Assert.True(MaxLength > 0);
    }

    private float CalculateTensionAmount(float distance)
    {
        float tension = distance / MaxLength;
        return Math.Clamp(tension, 0f, 1f);
    }

    private float CalculateOverstretch(float distance)
    {
        if (distance <= MaxLength)
            return 0f;
        return distance - MaxLength;
    }
}
