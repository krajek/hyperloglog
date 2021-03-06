﻿using System;
using System.Runtime.Serialization;

namespace HLLCardinalityEstimator
{
    [Serializable]
    public class HyperLogLogCore : IHyperLogLogCore, ISerializable
    {
        private byte[] _registers;

        private byte _b;

        private int _m;
        private double _alpha;

        /// <summary>
        /// Creates HyperLogLogCore instance.
        /// </summary>
        /// <param name="b">
        /// Number of bits of hash used to calculate register index. 
        /// There will be 2^b registers.
        /// The bigger the value of b the better accuraccy of count will be achieved.
        /// On the other hand, the greater the value of b, more memory will be used for the registers.
        /// </param>
        public HyperLogLogCore(byte b)
        {
            if (b < 4 || b > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(b), "Parameter 'b' must have value between 4 inclusive and 16 inclusive");
            }

            _b = b;
            _m = 1 << _b;
            _registers = new byte[_m];
            _alpha = HyperLogLogInternals.CalculateConstantAlphaCorrectionFactor(_b);
        }

        /// <summary>
        /// The constructor for deserialization.
        /// </summary>
        public HyperLogLogCore(SerializationInfo info, StreamingContext context)
        {
            _b = info.GetByte("b");
            _m = 1 << _b;
            _alpha = HyperLogLogInternals.CalculateConstantAlphaCorrectionFactor(_b);
            _registers = (byte[])info.GetValue("registers", typeof(byte[]));
        }

        /// <summary>
        /// Adds object's hash to the structure.
        /// Hash needs to be produced by a 'good' hash function.
        /// In practice it does not have to be cryptographycally secure.
        /// </summary>
        public void AddHash(ulong hash)
        {
            uint registerIndex = HyperLogLogInternals.CalculateRegisterIndex(hash, _b);
            byte proposedRegisterValue = HyperLogLogInternals.PositionOfLeftMostOne(hash, _b);
            byte newValueOfRegister = Math.Max(proposedRegisterValue, _registers[registerIndex]);
            _registers[registerIndex] = newValueOfRegister;
        }


        /// <summary>
        /// Returns estimated count of distict hashes added to HyperLogLogCore.
        /// There is not caching, estimated count is calculated on every call of this method.
        /// </summary>
        public int CalculateEstimatedCount()
        {
            double z = 0;
            for (int j = 0; j < _m; j++)
            {
                z += 1.0 / (1 << _registers[j]);
            }
            z = 1 / z;

            double rawEstimate = _alpha * _m * _m * z;

            return HyperLogLogInternals.AdjustForSmallOrBigRanges(rawEstimate, _registers);
        }

        /// <summary>
        /// Merges other HyperLogLogCore instance into the current one.
        /// This method does change the current instance, but it does not change the other one.
        /// </summary>
        public void Merge(HyperLogLogCore other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                throw new ArgumentException("Cannot merge instance of HyperLogLog to itself", nameof(other));
            }

            if (other._b != this._b)
            {
                throw new ArgumentException(
                    $"Cannot merge instance of HyperLogLog with b = {other._b} to instance with b = {this._b}",
                    nameof(other));
            }

            for (int i = 0; i < _m; i++)
            {
                _registers[i] = Math.Max(other._registers[i], _registers[i]);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("b", _b);
            info.AddValue("registers", _registers, typeof(byte[]));
        }
    }
}
