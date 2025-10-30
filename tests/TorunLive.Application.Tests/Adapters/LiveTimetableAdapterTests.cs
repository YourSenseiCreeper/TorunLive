using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using TorunLive.Application.Adapters;
using TorunLive.Application.Interfaces.Services;

namespace TorunLive.Application.Tests.Adapters
{
    [TestFixture]
    public class LiveTimetableAdapterTests
    {
        [Test]
        public void Parse_NewPageResponse_SucessfullyParse()
        {
            var testDateTime = new DateTimeOffset(2023, 11, 15, 21, 0, 0, TimeSpan.Zero);
            var dateTimeServiceMock = new Mock<IDateTimeService>();
            var service = new LiveTimetableAdapter(
                Mock.Of<ILogger<LiveTimetableAdapter>>(),
                dateTimeServiceMock.Object);

            var response = service.Adapt(Resource.liveNewResponse);

            response.ShouldNotBeNull();
            response.Lines.Count.ShouldBe(4);
        }
    }
}