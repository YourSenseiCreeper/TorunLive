using NUnit.Framework;
using Shouldly;
using TorunLive.Application.Parsers;

namespace TorunLive.Application.Tests.Sip
{
    public class LiveTimetableParserTests
    {
        [Test]
        public void Parse_NewPageResponse_SucessfullyParse()
        {
            var service = new LiveTimetableParser();
            var response = service.Parse(Resource.liveNewResponse);

            response.ShouldNotBeNull();
            response.Lines.ShouldSatisfyAllConditions(x => x.Count.ShouldBe(4));
        }
    }
}