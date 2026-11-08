using CincoVertice.Common.Utils.Helpers;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Helpers;

public class FloatingPointHelperTests
{
    public class DoubleAreEqual
    {
        [Theory]
        [InlineData(1.0, 1.0)]
        [InlineData(-1.0, -1.0)]
        [InlineData(1.234567, 1.234567)]
        public void IdenticalValues_DefaultTolerance_ReturnsTrue(double a, double b)
        {
            Assert.True(FloatingPointHelper.AreEqual(a, b));
        }

        [Fact]
        public void BothNull_ReturnsTrue()
        {
            Assert.True(FloatingPointHelper.AreEqual((double?)null, (double?)null));
        }

        [Theory]
        [InlineData(null, 0.0)]
        [InlineData(0.0, null)]
        public void OneNull_ReturnsFalse(double? a, double? b)
        {
            Assert.False(FloatingPointHelper.AreEqual(a, b));
        }

        [Theory]
        [InlineData(1.0, 1.0 + 0.000099999)]
        public void DifferenceWithinTolerance_ReturnsTrue(double a, double b)
        {
            Assert.True(FloatingPointHelper.AreEqual(a, b));
        }

        [Theory]
        [InlineData(1.0, 1.0 + 0.00011)]
        public void DifferenceExactlyTolerance_ReturnsFalse(double a, double b)
        {
            Assert.False(FloatingPointHelper.AreEqual(a, b));
        }

        [Fact]
        public void DifferenceOutsideTolerance_ReturnsFalse()
        {
            Assert.False(FloatingPointHelper.AreEqual(1.0, 1.0 + 0.0001001));
        }

        [Fact]
        public void CustomTolerance_Applies()
        {
            Assert.True(FloatingPointHelper.AreEqual(1.0, 1.0 + 1e-6, 1e-5));
            Assert.False(FloatingPointHelper.AreEqual(1.0, 1.0 + 1e-4, 1e-5));
        }

        [Fact]
        public void NegativeValuesWithinTolerance_ReturnsTrue()
        {
            Assert.True(FloatingPointHelper.AreEqual(-10.0, -10.0 + 0.00005));
        }

        [Fact]
        public void ZeroTolerance_IdenticalValues_ReturnsFalse()
        {
            Assert.False(FloatingPointHelper.AreEqual(2.0, 2.0, 0.0));
        }

        [Fact]
        public void NaN_And_Infinity_ReturnFalse_PerCurrentLogic()
        {
            Assert.False(FloatingPointHelper.AreEqual(double.NaN, double.NaN));
            Assert.False(FloatingPointHelper.AreEqual(double.NaN, 1.0));
            Assert.False(FloatingPointHelper.AreEqual(double.PositiveInfinity, double.PositiveInfinity));
            Assert.False(FloatingPointHelper.AreEqual(double.NegativeInfinity, double.NegativeInfinity));
            Assert.False(FloatingPointHelper.AreEqual(double.PositiveInfinity, double.NegativeInfinity));
        }
    }

    public class FloatAreEqual
    {
        [Theory]
        [InlineData(1f, 1f)]
        [InlineData(-1f, -1f)]
        public void IdenticalValues_DefaultTolerance_ReturnsTrue(float a, float b)
        {
            Assert.True(FloatingPointHelper.AreEqual(a, b));
        }

        [Fact]
        public void BothNull_ReturnsTrue()
        {
            Assert.True(FloatingPointHelper.AreEqual((float?)null, (float?)null));
        }

        [Theory]
        [InlineData(null, 0f)]
        [InlineData(0f, null)]
        public void OneNull_ReturnsFalse(float? a, float? b)
        {
            Assert.False(FloatingPointHelper.AreEqual(a, b));
        }

        [Fact]
        public void DifferenceWithinTolerance_ReturnsTrue()
        {
            Assert.True(FloatingPointHelper.AreEqual(1f, 1f + 0.0000999f));
        }

        [Fact]
        public void DifferenceExactlyTolerance_ReturnsFalse()
        {
            Assert.False(FloatingPointHelper.AreEqual(1f, 1f + 0.0001f));
        }

        [Fact]
        public void DifferenceOutsideTolerance_ReturnsFalse()
        {
            Assert.False(FloatingPointHelper.AreEqual(1f, 1f + 0.0001001f));
        }

        [Fact]
        public void NaN_And_Infinity_ReturnFalse_PerCurrentLogic()
        {
            Assert.False(FloatingPointHelper.AreEqual(float.NaN, float.NaN));
            Assert.False(FloatingPointHelper.AreEqual(float.NaN, 1f));
            Assert.False(FloatingPointHelper.AreEqual(float.PositiveInfinity, float.PositiveInfinity));
            Assert.False(FloatingPointHelper.AreEqual(float.NegativeInfinity, float.NegativeInfinity));
            Assert.False(FloatingPointHelper.AreEqual(float.PositiveInfinity, float.NegativeInfinity));
        }
    }
}
