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
    }
}
