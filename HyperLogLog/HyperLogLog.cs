using System;

namespace HyperLogLog
{
    public class HyperLogLog
    {
        private byte[] _registers;

        private byte _b;

        private int _m;

        /// <summary>
        /// Creates HyperLogLog instance.
        /// </summary>
        /// <param name="b">
        /// Number of bits of hash used to calculate register index. 
        /// There will be 2^b registers.
        /// The bigger the value of b the better accuraccy of count will be achieved.
        /// On the other hand, the greater the value of b, more memory will be used for the registers.
        /// </param>
        public HyperLogLog(byte b)
        {
            _b = b;
            _m = 1 << _b;
            _registers = new byte[_m];
        }

        public void AddHash(ulong hash)
        {
            uint registerIndex = HyperLogLogInternals.CalculateRegisterIndex(hash, _b);
            byte proposedRegisterValue = HyperLogLogInternals.CountLeadingZeroes(hash, _b);
            byte newValueOfRegister = Math.Max(proposedRegisterValue, _registers[registerIndex]);
            _registers[registerIndex] = newValueOfRegister;
        }

        public int CalculateEstimatedCount()
        {
            double z = 0;
            for (int j = 0; j < _m; j++)
            {
                z += 1.0 / (1 << _registers[j]);
            }
            z = 1 / z;

            int count = (int)((double)_m * (double)_m * z);

            return count;
        }
    }
}
