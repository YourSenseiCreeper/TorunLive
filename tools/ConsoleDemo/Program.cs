using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TorunLive.Application;
using TorunLive.Application.Interfaces.Services;

//var startStopId = 679;
//var dayOfWeek = PolishDayOfWeek.Friday;
//var dayMinute = 1352;
//service.GetTimetable(startStopId, dayOfWeek, dayMinute).GetAwaiter().GetResult();
//Console.WriteLine("Podaj id przystanku z systemu SIP: ");
//var sipStopId = int.Parse(Console.ReadLine());

//fullService.GetFullTimetable(sipStopId).GetAwaiter().GetResult();

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
            var directionName = AskForString($"Podaj nazwę kierunku dla lini $({defaultDirectionName}): ") ?? defaultDirectionName;
            fullService.GetLiveForLine("N90", "64301", "UNIWERSYTET").GetAwaiter().GetResult();
        }

        private static string? AskForString(string message)
        {
            Console.Write(message);
            var response = Console.ReadLine();
            return response;
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