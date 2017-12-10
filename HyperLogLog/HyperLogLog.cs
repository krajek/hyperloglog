using System;
using System.Security.Cryptography;

namespace HLLCardinalityEstimator
{
    public class HyperLogLog : HyperLogLogCore
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public HyperLogLog(
            HashAlgorithm hashAlgorithm,
            byte b) : base(b)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        public void AddInt32(int value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            ulong hash = HashBytes(valueBytes);
            base.AddHash(hash);
        }

        private ulong HashBytes(byte[] valueBytes)
        {
            byte[] hashBytes = _hashAlgorithm.ComputeHash(valueBytes);
            ulong hash = BitConverter.ToUInt64(hashBytes, 0);
            return hash;
        }
    }
}
