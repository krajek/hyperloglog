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
            HyperLogLog.HyperLogLog hyperLogLog=  new HyperLogLog.HyperLogLog(16);
            HashAlgorithm hashAlgorithm = SHA1.Create();
            ulong N = 1000000;
            for (ulong i = 0; i < N; i++)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(BitConverter.GetBytes(i));
                ulong hash = BitConverter.ToUInt64(hashBytes, 0);
                hyperLogLog.AddHash(hash);
            }

            int estimatedCount = hyperLogLog.CalculateEstimatedCount();

            double acceptableError = 0.01 * N;
            Assert.That(estimatedCount, Is.EqualTo(N).Within(acceptableError));
        }

    }
}
