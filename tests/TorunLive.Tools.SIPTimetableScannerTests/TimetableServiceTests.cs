using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TorunLive.SIPTimetableScanner.Interfaces;
using TorunLive.SIPTimetableScanner.Services;
using TorunLive.Tools.SIPTimetableScannerTests.Resources;

namespace TorunLive.Tools.SIPTimetableScannerTests
{
    public class TimetableServiceTests
    {
        [Test]
        public void ParseArrivals_Success()
        {
            var response = Resource.Response_ParseArrivals_Success;
            var services = new ServiceCollection();
            services.AddScoped<ITimetableParserService, TimetableParserService>();
            using (var provider = services.BuildServiceProvider())
            {
                var parser = provider.GetRequiredService<ITimetableParserService>();
                parser.ParseArrivals(39001, response);
            }
        }
    }
}
