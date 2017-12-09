using System;
using System.Security.Cryptography;
using HyperLogLog;
using NUnit.Framework;

namespace HyperLogLogTests
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

        [TestCase(1000, 16, 1)]
        [TestCase(10000, 16, 1)]
        [TestCase(10000, 4, 15)]
        [TestCase(1000000, 16, 1)]
        [TestCase(1000000, 4, 15)]
        public void CalculateEstimatedCount_ForGivenB_ShouldBeWithinExpectedError(int n, byte b, double acceptablePercentError)
        {
            var hyperLogLogCore = CreateHyperLogLogWithHashedIntegers(n, b);

            int estimatedCount = hyperLogLogCore.CalculateEstimatedCount();

            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

        [TestCase(1000, 16, 1)]
        [TestCase(10000, 16, 1)]
        [TestCase(10000, 10, 7)]
        [TestCase(1000000, 16, 1)]
        [TestCase(1000000, 4, 15)]
        public void Merge_DifferentSets_ShouldDoubletheEstimate(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b, start: n*10);
            first.Merge(second);

            // Act
            int estimatedCount = first.CalculateEstimatedCount();
            
            // Assert
            Assert.That(estimatedCount, Is.EqualTo(2*n).Within(acceptablePercentError).Percent);
        }

        [TestCase(1000, 16, 1)]
        [TestCase(10000, 16, 1)]
        [TestCase(10000, 10, 7)]
        [TestCase(1000000, 16, 1)]
        [TestCase(1000000, 4, 15)]
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

        private static HyperLogLogCore CreateHyperLogLogWithHashedIntegers(int n, byte b, int start = 0)
        {
            HyperLogLogCore hyperLogLogCore = new HyperLogLogCore(b);
            HashAlgorithm hashAlgorithm = MD5.Create();
            for (int i = start; i < start + n; i++)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(BitConverter.GetBytes(i));
                ulong hash = BitConverter.ToUInt64(hashBytes, 0);
                hyperLogLogCore.AddHash(hash);
            }
            return hyperLogLogCore;
        }
    }
}
