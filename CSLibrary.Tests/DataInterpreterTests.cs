using CSLibrary.V2.Stuff;
using System.Diagnostics;

namespace CSLibrary.Tests
{
    public class DataInterpreterTests
    {
        private DataInterpreter CreateWith(string data)
        {
            var di = new DataInterpreter();
            di.Data = data;
            return di;
        }

        [Fact]
        public void GetDate_ShouldCompleteWithinThreshold()
        {
            var data = "t=20251114T090552&s=123.45&fn=123456&fp=654321";
            var di = CreateWith(data);

            var sw = Stopwatch.StartNew();
            var dt = di.GetDate();
            sw.Stop();

            // порог в миллисекундах
            const long thresholdMs = 50;
            Assert.True(sw.ElapsedMilliseconds <= thresholdMs,
                $"GetDate took too long: {sw.ElapsedMilliseconds} ms (threshold {thresholdMs} ms)");
        }

        [Fact]
        public void GetDate_ShouldParseSeconds()
        {
            var data = "t=20251114T090523&s=123.45&fn=123456&fp=654321";
            var di = CreateWith(data);

            var dt = di.GetDate();

            Assert.Equal(2025, dt.Year);
            Assert.Equal(11, dt.Month);
            Assert.Equal(14, dt.Day);
            Assert.Equal(9, dt.Hour);
            Assert.Equal(5, dt.Minute);
            Assert.Equal(23, dt.Second);
        }

        [Fact]
        public void GetDate_ShouldParseDateAndTime_Correctly()
        {
            var data = "t=20251114T0905&s=123.45&fn=123456&fp=654321";
            var di = CreateWith(data);

            var dt = di.GetDate();

            Assert.Equal(2025, dt.Year);
            Assert.Equal(11, dt.Month);
            Assert.Equal(14, dt.Day);
            Assert.Equal(9, dt.Hour);
            Assert.Equal(5, dt.Minute);
            Assert.Equal(0, dt.Second);
        }

        [Fact]
        public void GetSum_ShouldReturnDecimal_WithCommaDecimalSeparator()
        {
            var data = "t=20251114T0905&s=123.45&fn=1&fp=2";
            var di = CreateWith(data);

            var sum = di.GetSum();

            Assert.Equal(123.45m, sum);
        }

        [Fact]
        public void GetFNNumber_ShouldReturnString()
        {
            var data = "t=20251114T0905&s=0&fn=ABC123&fp=2";
            var di = CreateWith(data);

            Assert.Equal("ABC123", di.GetFNNumber());
        }

        [Fact]
        public void GetFPNumber_ShouldReturnString()
        {
            var data = "t=20251114T0905&s=0&fn=1&fp=XYZ789";
            var di = CreateWith(data);

            Assert.Equal("XYZ789", di.GetFPNumber());
        }

        [Fact]
        public void SettingData_ShouldBeCaseInsensitiveForKeys()
        {
            var data = "T=20251114T090545&S=10.00&FN=aa&I=735209&FP=9119710006&N=1";
            var di = CreateWith(data);

            var date = di.GetDate();
            Assert.Equal(2025, date.Year);
            Assert.Equal(10.00m, di.GetSum());
            Assert.Equal("aa", di.GetFNNumber());
            Assert.Equal("9119710006", di.GetFPNumber());
        }

        [Fact]
        public void ParseData_WhenKeyMissing_ShouldThrowKeyNotFoundExceptionOnAccess()
        {
            var data = "t=20251114T0905&s=1.23&fn=1"; // fp missing
            var di = CreateWith(data);

            Assert.Throws<KeyNotFoundException>(() => _ = di.GetFPNumber());
        }

        [Fact]
        public void GetDate_WithInvalidFormat_ShouldThrow()
        {
            var data = "t=invalidtime&s=1&fn=1&fp=1";
            var di = CreateWith(data);

            Assert.ThrowsAny<Exception>(() => _ = di.GetDate());
        }

        [Fact]
        public void GetSum_WithInvalidNumberFormat_ShouldThrowFormatException()
        {
            var data = "t=20251114T0905&s=notanumber&fn=1&fp=1";
            var di = CreateWith(data);

            Assert.Throws<FormatException>(() => _ = di.GetSum());
        }

        [Fact]
        public void SettingData_MultipleCalls_ShouldAccumulateKeys_AndCauseArgumentExceptionOnDuplicate()
        {
            var di = new DataInterpreter();
            di.Data = "t=20251114T0905&s=1&fn=1&fp=1";

            Assert.Throws<ArgumentException>(() =>
                di.Data = "t=20251114T1000&s=2&fn=2&fp=2");
        }
    }
}
