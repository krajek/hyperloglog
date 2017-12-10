using System;
using System.Security.Cryptography;
using HLLCardinalityEstimator;
using Murmur;
using NUnit.Framework;

namespace HLLCardinalityEstimatorTests
{
    [TestFixture]
    public class HyperLogLogTests
    {
        [Test]
        public void CreatingHyperLogLogCore_WithCorrectParameterB_Succeeds([Range(4,16)] byte b)
        {
            Assert.DoesNotThrow(() => new HyperLogLogCore(b));
        }

        [Test]
        public void CreatingHyperLogLogCore_WithInCorrectParameterB_Succeeds([Values(3, 17)] byte b)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HyperLogLogCore(b));
        }

        // {n, b, expectedErrorInPercent}
        static object[] EstimationTestCases =
        {
            new object[] { 1000, (byte)16, 1.0 },
            new object[] { 10000, (byte)16, 1.0 },
            new object[] { 10000, (byte)10, 7.0 },
            new object[] { 1000000, (byte)16, 1.0 },
            new object[] { 1000000, (byte)4, 22.0 }
        };

        [TestCaseSource(nameof(EstimationTestCases))]
        public void CalculateEstimatedCount_ForGivenB_ShouldBeWithinExpectedError(int n, byte b, double acceptablePercentError)
        {
            var hyperLogLogCore = CreateHyperLogLogWithHashedIntegers(n, b);

            int estimatedCount = hyperLogLogCore.CalculateEstimatedCount();

            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_DifferentSets_ShouldDoubletheEstimate(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b, start: n);
            first.Merge(second);

            // Act
            int estimatedCount = first.CalculateEstimatedCount();
            
            // Assert
            Assert.That(estimatedCount, Is.EqualTo(2*n).Within(acceptablePercentError).Percent);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_SameSets_EstimateStaysTheSame(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b);
            first.Merge(second);

            // Act
            int estimatedCount = first.CalculateEstimatedCount();

            // Assert
            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_SetsWithHalfElementsTheSame_EstimateIs50PercentHigher(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b,n/2);
            first.Merge(second);

            // Act
            int estimatedCount = first.CalculateEstimatedCount();

            // Assert
            Assert.That(estimatedCount, Is.EqualTo(n*1.5).Within(acceptablePercentError).Percent);
        }

        private static HyperLogLog CreateHyperLogLogWithHashedIntegers(int n, byte b, int start = 0)
        {
            HyperLogLog hyperLogLog = new HyperLogLog(MurmurHash.Create128(), b);
            for (int i = start; i < start + n; i++)
            {
                hyperLogLog.AddInt32(i);
            }
            return hyperLogLog;
        }
    }
}
