using HyperLogLog;
using NUnit.Framework;

namespace HyperLogLogTests
{
    [TestFixture]
    public class HyperLogLogInternalsTests
    {
        [TestCase((uint)0, 6, ExpectedResult = 0)]
        [TestCase((uint)0, 16, ExpectedResult = 0)]
        [TestCase((uint)1, 6, ExpectedResult = 1)]
        [TestCase((uint)1, 16, ExpectedResult = 1)]
        [TestCase((uint)33, 5, ExpectedResult = 1, Description = "33 = 100001; First 5 bits = 00001 = 1")]
        [TestCase((uint)33, 6, ExpectedResult = 33, Description = "33 = 100001; First 6 bits = 100001 = 33")]
        [TestCase((uint)97, 6, ExpectedResult = 33, Description = "97 = 1100001; First 6 bits = 100001 = 33")]
        [TestCase((uint)99, 4, ExpectedResult = 3, Description = "99 = 1100011; First 4 bits = 0011 = 3")]
        public int CalculateRegisterIndexTest(uint hash, byte b)
        {
            return (int)HyperLogLogInternals.CalculateRegisterIndex(hash, b);
        }

        [TestCase((uint)0, 6, ExpectedResult = 58, Description = "64 bytes discarding 6 bytes = 58 bytes of zeroes")]
        [TestCase((uint)16, 4, ExpectedResult = 0, Description = "discarding 4 bytes and already 5th byte is 1, so there are not leading zeroes")]
        [TestCase((uint)32, 4, ExpectedResult = 1, Description = "discarding 4 bytes and 6th byte is 1, so there is one leading 0 at 5th byte")]
        public int CountLeadingZeroes(uint hash, byte b)
        {
            return HyperLogLogInternals.CountLeadingZeroes(hash, b);
        }
    }
}
