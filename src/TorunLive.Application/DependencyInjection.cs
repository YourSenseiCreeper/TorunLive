using Microsoft.Extensions.DependencyInjection;
using TorunLive.Application.Interfaces;
using TorunLive.Application.Interfaces.Parsers;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Parsers;
using TorunLive.Application.Services;

namespace TorunLive.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFullTimetableService, FullTimetableService>();
            services.AddScoped<ILiveTimetableService, LiveTimetableService>();
            services.AddScoped<ILiveTimetableParser, LiveTimetableParser>();
            services.AddScoped<ITimetableService, TimetableService>();
            services.AddScoped<ITimetableParser, TimetableParser>();
            services.AddScoped<ITimetableComparator, TimetableComparator>();
            services.AddScoped<ILineStopsService, LineStopsService>();
            return services;
        }
    }
}
