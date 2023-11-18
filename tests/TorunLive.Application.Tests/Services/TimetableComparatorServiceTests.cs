using NUnit.Framework;
using Shouldly;
using System.Linq;
using TorunLive.Application.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Tests.Services
{
    [TestFixture]
    public class TimetableComparatorServiceTests
    {
        [Test]
        public void Compare_OnlyOneLineOnlyOneArrival_NoErrorWithCorrectDelay()
        {
            const int currentDayMinute = 600;
            const int baseArrival = currentDayMinute + 3;
            const int liveArrival = currentDayMinute + 5;
            const string lineNumber = "10";
            var baseTimetable = new LiveTimetable()
            {
                Lines =
                [
                    new LiveLine { Number = lineNumber, ArrivalsInDayMinutes = [baseArrival] }
                ]
            };
            var liveTimetable = new LiveTimetable
            {
                Lines =
                [
                    new LiveLine { Number = lineNumber, ArrivalsInDayMinutes = [liveArrival] }
                ]
            };
            var service = new TimetableComparatorService();

            var result = service.Compare(baseTimetable, liveTimetable).ToList();

            result.Count.ShouldBe(1);
            result[0].Number.ShouldBe(lineNumber);
            result[0].Name.ShouldBe(null);
            result[0].Arrivals.Count.ShouldBe(1);
            result[0].Arrivals[0].BaseDayMinute.ShouldBe(baseArrival);
            result[0].Arrivals[0].Delay.ShouldBe(baseArrival - liveArrival);
            result[0].Arrivals[0].LiveDayMinute.ShouldBe(liveArrival);
        }

        [Test]
        public void Compare_TwoBaseLinesOneLiveLineOneArrival_OneErrorMessage()
        {
            const int currentDayMinute = 600;
            const int baseArrival = currentDayMinute + 3;
            const int liveArrival = currentDayMinute + 5;
            const string lineNumber = "10";
            var baseTimetable = new LiveTimetable()
            {
                Lines =
                [
                    new LiveLine { Number = lineNumber, ArrivalsInDayMinutes = [baseArrival] }
                ]
            };
            var liveTimetable = new LiveTimetable
            {
                Lines =
                [
                    new LiveLine { Number = lineNumber, ArrivalsInDayMinutes = [liveArrival] }
                ]
            };
            var service = new TimetableComparatorService();

            var result = service.Compare(baseTimetable, liveTimetable).ToList();

            result.Count.ShouldBe(1);
            result[0].Number.ShouldBe(lineNumber);
            result[0].Name.ShouldBe(null);
            result[0].Arrivals.Count.ShouldBe(1);
            result[0].Arrivals[0].BaseDayMinute.ShouldBe(baseArrival);
            result[0].Arrivals[0].Delay.ShouldBe(baseArrival - liveArrival);
            result[0].Arrivals[0].LiveDayMinute.ShouldBe(liveArrival);
        }
    }
}
