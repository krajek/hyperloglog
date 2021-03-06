﻿using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Murmur;

namespace HLLCardinalityEstimator
{
    [Serializable]
    public class HyperLogLog : HyperLogLogCore, IHyperLogLog
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public HyperLogLog(byte b) : base(b)
        {
            _hashAlgorithm = MurmurHash.Create128();
        }

        public HyperLogLog(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _hashAlgorithm = MurmurHash.Create128();
        }

        public void AddByte(byte value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            AddBytes(valueBytes);
        }

        public void AddInt16(Int16 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            AddBytes(valueBytes);
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
