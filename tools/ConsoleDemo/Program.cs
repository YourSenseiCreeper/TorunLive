using ConsoleTables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TorunLive.Application;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Extensions;

namespace ConsoleDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddRepositories();
            builder.Services.AddServices();
            using IHost host = builder.Build();

            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var fullService = provider.GetRequiredService<IFullTimetableService>();

            var defaultLineName = "N90";
            var defaultStartStopId = 64301;
            var defaultDirectionName = "UNIWERSYTET";
            var lineName = AskForString($"Podaj nazwę lini ({defaultLineName}): ") ?? defaultLineName;
            var startStopId = AskForInt($"Id przystanku startowego ({defaultStartStopId}): ") ?? defaultStartStopId;
            var directionName = AskForString($"Podaj nazwę kierunku dla lini ({defaultDirectionName}): ") ?? defaultDirectionName;
            var result = fullService.GetLiveForLine(lineName, startStopId.ToString(), directionName).GetAwaiter().GetResult();

            var table = new ConsoleTable("Przystanek", "Planowy", "Aktualny", "Opóźnienie");
            foreach(var arrival in result.Arrivals)
            {
                table.Rows.Add(new object[]
                {
                    arrival.StopName,
                    arrival.BaseDayMinute.GetDateTimeFromDayMinute(),
                    arrival.ActualBaseMinute.HasValue ? arrival.ActualBaseMinute.Value.GetDateTimeFromDayMinute() : "brak",
                    arrival.Delay.HasValue ? arrival.Delay : "-"
                });
            }

            table.Write();
        }


        private static string? AskForString(string message)
        {
            Console.Write(message);
            var response = Console.ReadLine();
            return string.IsNullOrEmpty(response) ? null : response;
        }

        private static int? AskForInt(string message)
        {
            Console.Write(message);
            var response = Console.ReadLine();
            if (string.IsNullOrEmpty(response) || !int.TryParse(response, out int parsedResponse))
                return null;

            return parsedResponse;
        }
    }
}