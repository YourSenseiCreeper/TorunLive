﻿using ConsoleTables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TorunLive.Application;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using TorunLive.Persistance;

namespace ConsoleDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Services.AddServices();
            builder.Services.AddDbContext<TorunLiveContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(ConfigurationKeys.ConnectionString))
            );

            using IHost host = builder.Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var fullService = provider.GetRequiredService<IFullTimetableService>();

            var defaultLineName = "27";
            var defaultStartStopId = 29102;
            var defaultDirectionId = 1;
            var lineName = defaultLineName;
            var startStopId = defaultStartStopId;
            var directionId = defaultDirectionId;
            //var lineName = AskForString($"Podaj nazwę lini ({defaultLineName}): ") ?? defaultLineName;
            //var startStopId = AskForInt($"Id przystanku startowego ({defaultStartStopId}): ") ?? defaultStartStopId;
            //var directionId = AskForInt($"Podaj id kierunku dla lini ({defaultDirectionId}): ") ?? defaultDirectionId;
            var result = fullService.GetLiveForLine(lineName, startStopId.ToString(), directionId).GetAwaiter().GetResult();

            if (result.IsFailure)
            {
                Console.WriteLine(result.ErrorMessage);
                return;
            }

            var table = new ConsoleTable("Przystanek", "Planowany", "Aktualny", "Opóźnienie");
            table.Configure(options => options.EnableCount = false);
            var now = DateTime.Now;
            foreach(var arrival in result.ResultObject.Arrivals)
            {
                table.Rows.Add(new object[]
                {
                    arrival.StopName,
                    $"{arrival.BaseDayMinute.GetDateTimeFromDayMinute(now):HH:mm}",
                    arrival.LiveDayMinute.HasValue ? $"{arrival.LiveDayMinute.Value.GetDateTimeFromDayMinute(now):HH:mm}" : "brak",
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