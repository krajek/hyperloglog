using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HyperLogLog
{
    public class HyperLogLog
    {
        private readonly HashAlgorithm _hashAlgorithm;
        private byte[] _registers;

        private byte _b;

        /// <summary>
        /// Creates HyperLogLog instance.
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="b">
        /// Number of bits of hash used to calculate register index. 
        /// There will be 2^b registers.
        /// The bigger the value of b the better accuraccy of count will be achieved.
        /// On the other hand, the greater the value of b, more memory will be used for the registers.
        /// </param>
        public HyperLogLog(byte b)
        {
            _b = b;
        }

        public void AddHash(ulong hash)
        {
            uint registerIndex = HyperLogLogInternals.CalculateRegisterIndex(hash, _b);
            byte proposedRegisterValue = HyperLogLogInternals.CountLeadingZeroes(hash, _b);
            _registers[registerIndex] = Math.Max(proposedRegisterValue, _registers[registerIndex]);
        }

        public int CalculateEstimatedCount()
        {
            return 0;
        }
    }
}
