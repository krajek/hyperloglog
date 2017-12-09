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

        [TestCase((uint)0, 6, ExpectedResult = 59, Description = "64 bytes discarding 6 bytes = 58 bytes of zeroes")]
        [TestCase((uint)16, 4, ExpectedResult = 1, Description = "discarding 4 bytes and already 5th byte is 1, so there are not leading zeroes")]
        [TestCase((uint)32, 4, ExpectedResult = 2, Description = "discarding 4 bytes and 6th byte is 1, so there is one leading 0 at 5th byte")]
        [TestCase((uint)64, 4, ExpectedResult = 3, Description = "discarding 4 bytes and 7th byte is 1, so there are two leading 0 at 5th and 6th byte")]
        [TestCase((uint)65, 4, ExpectedResult = 3, Description = "same as previous, first byte should have no meaning, discarding 4 bytes and 7th byte is 1, so there are two leading 0 at 5th and 6th byte")]
        public int PositionOfLeftMostOne(uint hash, byte b)
        {
            return HyperLogLogInternals.PositionOfLeftMostOne(hash, b);
        }

        [TestCase(4, ExpectedResult = 0.673, Description = "Constant specified in HLL paper")]
        [TestCase(5, ExpectedResult = 0.697, Description = "Constant specified in HLL paper")]
        [TestCase(6, ExpectedResult = 0.709, Description = "Constant specified in HLL paper")]
        public double CalculateConstantAlphaCorrectionFactor_Constants(byte b)
        {
            return HyperLogLogInternals.CalculateConstantAlphaCorrectionFactor(b);
        }

        [Test]
        public void CalculateConstantAlphaCorrectionFactor_CalculatedForParamB7()
        {
            double result = HyperLogLogInternals.CalculateConstantAlphaCorrectionFactor(7);

            // b = 7 => m = 128
            // 0,7213 / (1 + (1,079 / 128)) = 0,715270493
            Assert.That(result, Is.EqualTo(0.715270493).Within(0.000000001));
        }

        
    }
}
