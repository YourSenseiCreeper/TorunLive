using NUnit.Framework;
using Shouldly;
using System.Linq;
using TorunLive.SIPTimetableScanner.Services;
using TorunLive.Tools.SIPTimetableScannerTests.Resources;

namespace TorunLive.Tools.SIPTimetableScannerTests
{
    public class Tests
    {
        [Test]
        public void ParseArrivals_Success()
        {
            var service = new TimetableParserService();
            var lineStopId = 1;
            var firstArrivalDayMinute = 281;
            var result = service.ParseArrivals(lineStopId, Resource.Response_ParseArrivals_Success).ToList();
            result.ShouldNotBeEmpty();
            result[0].LineStopId.ShouldBe(lineStopId);
            result[0].DayMinute.ShouldBe(firstArrivalDayMinute);
            result[0].IsWeekday.ShouldBeTrue();
            result[0].IsWinterHoliday.ShouldBeFalse();
            result[0].IsSaturdaySundays.ShouldBeFalse();
            result[0].IsHolidays.ShouldBeFalse();
        }

        [Test]
        public void ParseTimetablesUrls_Success()
        {
            Assert.Pass();
        }
    }
}