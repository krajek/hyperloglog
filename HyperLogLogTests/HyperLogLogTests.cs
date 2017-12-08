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
        public void EstimatingCardinalityFor10000000ElementsShouldBeWithinFewPercentError()
        {
            HyperLogLog.HyperLogLogCore hyperLogLogCore=  new HyperLogLog.HyperLogLogCore(16);
            HashAlgorithm hashAlgorithm = SHA1.Create();
            ulong N = 1000000;
            for (ulong i = 0; i < N; i++)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(BitConverter.GetBytes(i));
                ulong hash = BitConverter.ToUInt64(hashBytes, 0);
                hyperLogLogCore.AddHash(hash);
            }

            int estimatedCount = hyperLogLogCore.CalculateEstimatedCount();

            double acceptableError = 0.01 * N;
            Assert.That(estimatedCount, Is.EqualTo(N).Within(acceptableError));
        }

    }
}
