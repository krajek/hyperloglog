using System;
using System.Globalization;
using HLLCardinalityEstimator;
using Murmur;
using NUnit.Framework;

namespace HLLCardinalityEstimatorTests
{
    [TestFixture]
    public class HyperLogLogTests
    {
        [Test]
        public void CreatingHyperLogLogCore_WithCorrectParameterB_Succeeds([Range(4, 16)] byte b)
        {
            Assert.DoesNotThrow(() => new HyperLogLogCore(b));
        }

        [Test]
        public void CreatingHyperLogLogCore_WithInCorrectParameterB_Succeeds([Values(3, 17)] byte b)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new HyperLogLogCore(b));
        }

        // {n, b, expectedErrorInPercent}
        static object[] EstimationTestCases =
        {
            new object[] {1000, (byte) 16, 1.0},
            new object[] {10000, (byte) 16, 1.0},
            new object[] {10000, (byte) 10, 7.0},
            new object[] {1000000, (byte) 16, 1.0},
            new object[] {1000000, (byte) 4, 22.0}
        };

        [TestCaseSource(nameof(EstimationTestCases))]
        public void CalculateEstimatedCount_Int32_ShouldBeWithinExpectedError(
            int n, 
            byte b,
            double acceptablePercentError)
        {
            Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
                CreateHyperLogLogWithHashedIntegers(n, b),
                n, 
                acceptablePercentError);
        }

        [TestCase(100000, 16, 1.0)]
        public void CalculateEstimatedCount_String_ShouldBeWithinExpectedError(int n, byte b,double acceptablePercentError)
        {
            // Arrange
            var hyperLogLogCore = CreateHyperLogLogWithHashedStrings(n, b);

            // Act
            int estimatedCount = hyperLogLogCore.CalculateEstimatedCount();

            // Assert
            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

        [TestCase(10000, 16, 1.0)]
        public void CalculateEstimatedCount_Int16_ShouldBeWithinExpectedError(short n, byte b, double acceptablePercentError)
        {
            Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
                CreateHyperLogLogWithHashedIntegers16(n, b),
                n,
                acceptablePercentError);
        }

        [TestCase(100000, 16, 1.0)]
        public void CalculateEstimatedCount_Int64_ShouldBeWithinExpectedError(int n, byte b, double acceptablePercentError)
        {
            Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
                CreateHyperLogLogWithHashedIntegers64(n, b),
                n,
                acceptablePercentError);
        }

        [TestCase(100000, 16, 1.0)]
        public void CalculateEstimatedCount_Guid_ShouldBeWithinExpectedError(int n, byte b, double acceptablePercentError)
        {
            Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
                CreateHyperLogLogWithHashedGuids(n, b),
                n,
                acceptablePercentError);
        }

        [TestCase(100000, 16, 1.0)]
        public void CalculateEstimatedCount_DateTime_ShouldBeWithinExpectedError(int n, byte b, double acceptablePercentError)
        {
            Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
                CreateHyperLogLogWithHashedDateTimes(n, b),
                n,
                acceptablePercentError);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_DifferentSets_ShouldDoubletheEstimate(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b, start: n);

            // Act
            first.Merge(second);

            // Assert
            int estimatedCount = first.CalculateEstimatedCount();
            Assert.That(estimatedCount, Is.EqualTo(2 * n).Within(acceptablePercentError).Percent);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_SameSets_EstimateStaysTheSame(int n, byte b, double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b);

            // Act
            first.Merge(second);

            // Assert
            int estimatedCount = first.CalculateEstimatedCount();
            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }

        [TestCaseSource(nameof(EstimationTestCases))]
        public void Merge_SetsWithHalfElementsTheSame_EstimateIs50PercentHigher(int n, byte b,
            double acceptablePercentError)
        {
            // Arrange
            var first = CreateHyperLogLogWithHashedIntegers(n, b);
            var second = CreateHyperLogLogWithHashedIntegers(n, b, n / 2);

            // Act
            first.Merge(second);

            // Assert
            int estimatedCount = first.CalculateEstimatedCount();
            Assert.That(estimatedCount, Is.EqualTo(n * 1.5).Within(acceptablePercentError).Percent);
        }

        [Test]
        public void Merge_NullSet_ThrowsExeption()
        {
            // Arrange
            var hyperLogLog = new HyperLogLog(4);

            // Act
            TestDelegate action = () => hyperLogLog.Merge(null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("other", exception.ParamName);
        }

        [Test]
        public void Merge_SameSet_ThrowsExeption()
        {
            // Arrange
            var hyperLogLog = new HyperLogLog(4);

            // Act
            TestDelegate action = () => hyperLogLog.Merge(hyperLogLog);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.AreEqual("other", exception.ParamName);
            StringAssert.StartsWith("Cannot merge instance of HyperLogLog to itself", exception.Message);
        }

        [Test]
        public void Merge_DifferentB_ThrowsExeption()
        {
            // Arrange
            var hyperLogLog = new HyperLogLog(4);
            var other = new HyperLogLog(5);

            // Act
            TestDelegate action = () => hyperLogLog.Merge(other);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.AreEqual("other", exception.ParamName);
            StringAssert.StartsWith("Cannot merge instance of HyperLogLog with b = 5 to instance with b = 4", exception.Message);
        }

        private static HyperLogLog CreateHyperLogLogWithHashedIntegers(int n, byte b, int start = 0)
        {
            var hyperLogLog = new HyperLogLog(b);
            for (Int32 i = start; i < start + n; i++)
            {
                hyperLogLog.AddInt32(i);
            }
            return hyperLogLog;
        }

        private static HyperLogLog CreateHyperLogLogWithHashedIntegers16(short n, byte b)
        {
            var hyperLogLog = new HyperLogLog(b);
            for (Int16 i = 0; i < n; i++)
            {
                hyperLogLog.AddInt64(i);
            }
            return hyperLogLog;
        }

        private static HyperLogLog CreateHyperLogLogWithHashedIntegers64(int n, byte b, int start = 0)
        {
            var hyperLogLog = new HyperLogLog(b);
            for (Int64 i = start; i < start + n; i++)
            {
                hyperLogLog.AddInt64(i);
            }
            return hyperLogLog;
        }

        private static HyperLogLog CreateHyperLogLogWithHashedGuids(int n, byte b, int start = 0)
        {
            var hyperLogLog = new HyperLogLog(b);
            for (Int64 i = start; i < start + n; i++)
            {
                hyperLogLog.AddGuid(Guid.NewGuid());
            }
            return hyperLogLog;
        }

        private static HyperLogLog CreateHyperLogLogWithHashedDateTimes(int n, byte b, int start = 0)
        {
            var hyperLogLog = new HyperLogLog(b);

            for (Int64 i = start; i < start + n; i++)
            {
                hyperLogLog.AddDateTime(new DateTime(2000, 1, 1).AddMinutes(i));
            }
            return hyperLogLog;
        }

        private static HyperLogLog CreateHyperLogLogWithHashedStrings(int n, byte b)
        {
            HyperLogLog hyperLogLog = new HyperLogLog(b);
            for (int i = 0; i < n; i++)
            {
                hyperLogLog.AddUTF8String(i.ToString(CultureInfo.InvariantCulture));
            }

            return hyperLogLog;
        }

        private static void Test_CalculateEstimatedCount_ShouldBeWithinAcceptableErrorRange(
            HyperLogLog hyperLogLog,
            int n, 
            double acceptablePercentError)
        {
            // Act
            int estimatedCount = hyperLogLog.CalculateEstimatedCount();

            // Assert
            Assert.That(estimatedCount, Is.EqualTo(n).Within(acceptablePercentError).Percent);
        }
    }
}
