using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using TorunLive.Application.Adapters;

namespace TorunLive.Application.Tests
{
    [TestFixture]
    public class LiveTimetableAdapterTests
    {
        [Test]
        public void Parse_NewPageResponse_SucessfullyParse()
        {
            var service = new LiveTimetableAdapter(Mock.Of<ILogger<LiveTimetableAdapter>>());
            var response = service.Adapt(Resource.liveNewResponse);

            response.ShouldNotBeNull();
            response.Lines.Count.ShouldBe(4);
        }
    }
}