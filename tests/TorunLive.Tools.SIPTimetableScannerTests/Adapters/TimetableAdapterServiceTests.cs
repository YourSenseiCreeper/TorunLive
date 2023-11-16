using NUnit.Framework;
using Shouldly;
using System.Linq;
using TorunLive.SIPTimetableScanner.Adapters;
using TorunLive.Tools.SIPTimetableScannerTests.Resources;

namespace TorunLive.Tools.SIPTimetableScannerTests.Adapters
{
    [TestFixture]
    public class TimetableAdapterServiceTests
    {
        [Test]
        [Ignore("To be fixed")]
        public void ParseArrivals_Success()
        {
            var service = new TimetableAdapterService();
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
    }
}