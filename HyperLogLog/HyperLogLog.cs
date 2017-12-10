using System;
using System.Security.Cryptography;
using System.Text;

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

        public void AddInt32(Int32 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            AddBytes(valueBytes);
        }

        public void AddInt64(Int64 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            AddBytes(valueBytes);
        }

        public void AddUTF8String(string value)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            AddBytes(valueBytes);
        }

        public void AddGuid(Guid value)
        {
            byte[] valueBytes = value.ToByteArray();
            AddBytes(valueBytes);
        }

        public void AddDateTime(DateTime value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value.ToBinary());
            AddBytes(valueBytes);
        }

        public void AddBytes(byte[] valueBytes)
        {
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
