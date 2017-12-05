namespace HyperLogLog
{
    public static class HyperLogLogInternals
    {
        public static uint CalculateRegisterIndex(ulong hash, byte b)
        {
            ulong mask = (ulong)(1 << b) - 1;
            ulong registerIndex = hash & mask;
            return (uint) registerIndex;
        }

        public static byte CountLeadingZeroes(ulong hash, byte b)
        {
            for (byte bitIndex = b; bitIndex < 63; bitIndex++)
            {
                ulong maskedHash = (hash >> bitIndex) & 1;
                if (maskedHash != 0)
                {
                    byte leadingZeroes = (byte) (bitIndex - b);
                    return leadingZeroes;
                }
            }

            return (byte) (64 - b);
        }
    }
}
