using System;

namespace HyperLogLog
{
    public static class HyperLogLogInternals
    {
        /// <summary>
        /// Returns integer representation of first b bits in hash.
        /// </summary>
        public static uint CalculateRegisterIndex(ulong hash, byte b)
        {
            ulong mask = (ulong)(1 << b) - 1;
            ulong registerIndex = hash & mask;
            return (uint) registerIndex;
        }

        /// <summary>
        /// Returns 1-based position of first 1 bit in hash after discarding first b bits.
        /// Described in HLL paper as:
        /// "let ρ(s) be the position of the leftmost 1-bit of s: e.g., ρ(1 · · ·) = 1, ρ(0001 · · ·) = 4, ρ(0^K) = K + 1"
        /// </summary>
        public static byte PositionOfLeftMostOne(ulong hash, byte b)
        {
            for (byte bitIndex = b; bitIndex <= 63; bitIndex++)
            {
                ulong maskedHash = (hash >> bitIndex) & (ulong)1;
                if (maskedHash != 0)
                {
                    byte leadingZeroes = (byte) (bitIndex - b + 1);
                    return leadingZeroes;
                }
            }

            return (byte) (64 - b + 1);
        }

        /// <summary>
        /// Returns alpha factor as specified in HLL paper:
        /// "define α16 = 0.673; α32 = 0.697; α64 = 0.709; αm = 0.7213/(1+ 1.079/m) for m ≥ 128;"
        /// </summary>
        public static double CalculateConstantAlphaCorrectionFactor(byte b)
        {
            switch (b)
            {
                case 4: return 0.673;
                case 5: return 0.697;
                case 6: return 0.709;
                default: return 0.7213 / (1.0 + (1.079 / (1 << b)));
            }
        }

        public static int AdjustForSmallOrBigRanges(double rawEstimate, byte[] registers)
        {
            int m = registers.Length;

            const double twoToThePowerOf32 = 4294967296.0;
            const double bigRangeThreshold = twoToThePowerOf32 / 30.0; // around 143 000 000
            
            if (rawEstimate <= 2.5 * m)
            {
                // small range correction
                int zeroRegisters = 0;
                for (int i = 0; i < m; i++)
                {
                    if (registers[i] == 0)
                    {
                        zeroRegisters++;
                    }
                }

                if (zeroRegisters == 0)
                {
                    return (int)rawEstimate;
                }
                else
                {
                    return (int)(m * Math.Log((double)m / (double)zeroRegisters));
                }
            }
            else if (rawEstimate <= bigRangeThreshold)
            {
                // intermediate range: no correction
                return (int)rawEstimate;
            }
            else
            {
                // large range correction
                double largeRangeCorrectedResult = -twoToThePowerOf32 * Math.Log(1 - rawEstimate / twoToThePowerOf32);
                return (int)largeRangeCorrectedResult;
            }
        }


    }
}
