using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TorunLive.Persistance;
using TorunLive.Application;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using TorunLive.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TorunLive.WebApi.IntegrationTests
{
    public class TimetableControllerTests
    {
        [Test]
        public async Task GET_RetrievesLineDelaysCompareToTimetable_Success()
        {
            // tak skonfigurować, aby nie wysyłał requestów do zewnętrznych serwisów
            await using var application = new WebApplicationFactory<WebApi.Program>();
            var client = application.CreateClient();

            int lineNumber = 27;
            string sipStopId = "29102";
            int directionId = 1;
            var url = $"/Timetable/GetDelay?lineNumber={lineNumber}&sipStopId={sipStopId}&directionId={directionId}";

            var request = await client.GetAsync(url);

            Assert.That(request.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var response = await request.Content.ReadFromJsonAsync<CompareLine>();

            Assert.That(response, Is.Not.Null);
        }
    }
}