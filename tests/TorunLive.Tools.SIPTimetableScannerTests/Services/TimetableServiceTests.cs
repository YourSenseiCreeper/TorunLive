using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner;
using TorunLive.SIPTimetableScanner.Adapters;
using TorunLive.SIPTimetableScanner.Entities;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Services;
using TorunLive.SIPTimetableScanner.Services;
using TorunLive.Tools.SIPTimetableScannerTests.Resources;

namespace TorunLive.Tools.SIPTimetableScannerTests.Services
{
    [TestFixture]
    public class TimetableServiceTests
    {
        [Test]
        [Ignore("To be fixed")]
        public void ParseArrivals_Success()
        {
            var response = Resource.Response_ParseArrivals_Success;
            var services = new ServiceCollection();
            services.AddScoped<ITimetableAdapterService, TimetableAdapterService>();
            using (var provider = services.BuildServiceProvider())
            {
                var parser = provider.GetRequiredService<ITimetableAdapterService>();
                parser.ParseArrivals(39001, response);
            }
        }

        [Test]
        [Ignore("To be fixed")]
        public async Task ScanLineDirectionStopAndStopTimes_Success()
        {
            var services = new ServiceCollection();
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c[ConfigurationKeys.RequestDelayBaseInMiliseconds]).Returns("0");
            services.AddSingleton(configuration.Object);

            var requestMock = new Mock<IRequestService>();
            requestMock.Setup(r => r.GetTimetable(It.IsAny<string>())).ReturnsAsync(Resource.Response_ParseTimetablesUrls_Success);
            requestMock.Setup(r => r.GetTimetable("1", 1, "101")).ReturnsAsync(Resource.Response_ParseTimetablesUrls_Success);
            services.AddSingleton(requestMock.Object);
            services.AddSingleton<ITimetableAdapterService, TimetableAdapterService>();
            services.AddSingleton(Mock.Of<IDelayService>());
            services.AddSingleton(Mock.Of<ILogger<TimetableService>>());
            services.AddScoped<ITimetableService, TimetableService>();
            services.AddDbContext<TorunLiveContext>(options =>
                options.UseInMemoryDatabase("TorunLive")
            );

            using (var provider = services.BuildServiceProvider())
            {
                var service = provider.GetRequiredService<ITimetableService>();
                var urls = new List<string> { "1/1/101.html", "1/2/101.html" };
                var direction = new LineDirection { Url = urls[0], LineId = "1", DirectionName = "Prawobrzeże" };
                await service.ScanLineDirectionStopAndStopTimes(direction, urls);

                var dbContext = provider.GetRequiredService<TorunLiveContext>();
                var stops = await dbContext.Stops.ToListAsync();
                stops.Count.ShouldBe(1);
                stops[0].Id.ShouldBe("1");

                var directions = await dbContext.Directions.ToListAsync();
                directions.Count.ShouldBe(1);
                directions[0].Name.ShouldBe("Prawobrzeże");

                var lineStops = await dbContext.LineStops.ToListAsync();
                lineStops.Count.ShouldBe(1);

                var lineStopTimes = await dbContext.LineStopTimes.ToListAsync();
                lineStopTimes.Count.ShouldBe(23);
            }
        }
    }
}
