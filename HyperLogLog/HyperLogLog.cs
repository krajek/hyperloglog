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

        public void Add(int value)
        {
            byte[] hashBytes = _hashAlgorithm.ComputeHash(BitConverter.GetBytes(value));
            ulong hash = BitConverter.ToUInt64(hashBytes, 0);
            base.AddHash(hash);
        }
    }
}
