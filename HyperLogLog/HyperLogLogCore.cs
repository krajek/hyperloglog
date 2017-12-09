using System;

namespace HyperLogLog
{
    public class HyperLogLogCore
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
            _alpha = HyperLogLogInternals.CalculateConstantAlphaCorrectionFactor(b);
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

        public int CalculateEstimatedCount()
        {
            double z = 0;
            for (int j = 0; j < _m; j++)
            {
                z += 1.0 / (1 << _registers[j]);
            }
            z = 1 / z;

            int count = (int)(_alpha * _m * _m * z);

            return count;
        }
    }
}
