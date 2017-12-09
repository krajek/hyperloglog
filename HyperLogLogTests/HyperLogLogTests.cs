using System;
using System.Security.Cryptography;
using HyperLogLog;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
            HyperLogLogCore hyperLogLogCore = new HyperLogLogCore(b);
            HashAlgorithm hashAlgorithm = MD5.Create();
            for (int i = 0; i < n; i++)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(BitConverter.GetBytes(i));
                ulong hash = BitConverter.ToUInt64(hashBytes, 0);
                hyperLogLogCore.AddHash(hash);
            }

            int estimatedCount = hyperLogLogCore.CalculateEstimatedCount();

            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

    }
}
