using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using TorunLive.Application.Adapters;
using TorunLive.Application.Extensions;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Services;
using TorunLive.Domain.Database;
using TorunLive.Persistance;

namespace TorunLive.Application.Tests
{
    [TestFixture]
    public class FullTimetableServiceTests
    {
        [Test]
        [Ignore("To be fixed")]
        public async Task GetFullTimetable_Success()
        {
            var testDateTime = new DateTimeOffset(2023, 11, 15, 21, 0, 0, TimeSpan.Zero);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.SetupGet(dt => dt.Now).Returns(testDateTime);

            var services = new ServiceCollection();
            var requestMock = new Mock<ILiveRequestService>();
            requestMock.Setup(r => r.GetTimetable(It.IsAny<string>())).ReturnsAsync(Resource.liveNewResponse);
            services.AddSingleton(requestMock.Object);
            services.AddSingleton(dateTimeServiceMock.Object);
            services.AddSingleton<ITimetableComparatorService, TimetableComparatorService>();
            services.AddSingleton(Mock.Of<ILogger<LiveTimetableAdapter>>());
            services.AddSingleton<ILiveTimetableAdapter, LiveTimetableAdapter>();
            services.AddScoped<IFullTimetableService, FullTimetableService>();
            services.AddDbContext<TorunLiveContext>(options =>
                options.UseInMemoryDatabase(ConfigurationKeys.ConnectionString)
            );

            using var provider = services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<TorunLiveContext>();
            string sipStopId = "64301";
            var stop = new Stop { Id = sipStopId, Name = "Bednarska 01" };
            dbContext.Stops.Add(stop);
            var testDateTimeInDayMinute = testDateTime.ToDayMinute();
            AddAllLineStopData(dbContext, "10", 1, "Daleka", sipStopId, [testDateTimeInDayMinute + 3]);
            AddAllLineStopData(dbContext, "13", 1, "Wysoka", sipStopId, [testDateTimeInDayMinute + 5]);
            AddAllLineStopData(dbContext, "21", 2, "Bielany wyb.", sipStopId, [testDateTimeInDayMinute + 7]);
            var service = provider.GetRequiredService<IFullTimetableService>();

            var response = await service.GetFullTimetable(sipStopId);

            response.ShouldNotBeNull();
            response.ShouldNotBeEmpty();
        }

        private void AddAllLineStopData(
            TorunLiveContext dbContext,
            string lineId,
            int directionId,
            string directionName,
            string stopId,
            int[] arrivalDayMintues)
        {
            var line = new Line { Id = lineId };
            dbContext.Lines.Add(line);

            var direction = new Direction { LineId = lineId, DirectionId = directionId, Name = directionName };
            dbContext.Directions.Add(direction);

            var lineStop = new LineStop
            {
                DirectionId = directionId,
                DirectionLineId = line.Id,
                LineId = line.Id,
                StopId = stopId,
                StopOrder = 5,
                IsOnDemand = false,
            };
            dbContext.LineStops.Add(lineStop);

            dbContext.LineStopTimes.AddRange(arrivalDayMintues.Select(arrival => new LineStopTime
            {
                LineStop = lineStop,
                IsWeekday = true,
                IsSaturdaySundays = false,
                IsWinterHoliday = false,
                IsHolidays = false,
                DayMinute = arrival,
            }));
            dbContext.SaveChanges();
        }
    }
}