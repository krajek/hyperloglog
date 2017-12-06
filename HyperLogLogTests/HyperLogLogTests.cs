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
        public void EstimatingCardinalityFor100000ElementsShouldBeWithinFewPercentError()
        {
            HyperLogLog.HyperLogLog hyperLogLog=  new HyperLogLog.HyperLogLog(16);
            HashAlgorithm hashAlgorithm = new SHA256Cng();
            
            for (int i = 0; i < 100000; i++)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(BitConverter.GetBytes(i));
                ulong hash = BitConverter.ToUInt64(hashBytes, 0);
                hyperLogLog.AddHash(hash);
            }

            int estimatedCount = hyperLogLog.CalculateEstimatedCount();

            Assert.Greater(estimatedCount, 95000);
            Assert.Less(estimatedCount, 105000);
        }

    }
}
