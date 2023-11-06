using Microsoft.Extensions.DependencyInjection;
using TorunLive.Application.Interfaces.Parsers;
using TorunLive.Application.Interfaces.Repositories;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Parsers;
using TorunLive.Application.Repositories;
using TorunLive.Application.Services;

namespace TorunLive.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ILineStopsRepository, LineStopsRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFullTimetableService, FullTimetableService>();
            services.AddScoped<ILiveTimetableService, LiveTimetableService>();
            services.AddScoped<ILiveTimetableParser, LiveTimetableParser>();
            services.AddScoped<ITimetableService, TimetableService>();
            services.AddScoped<ITimetableParser, TimetableParser>();
            services.AddScoped<ITimetableComparatorService, TimetableComparatorService>();
            services.AddScoped<ILineStopsService, LineStopsService>();
            return services;
        }
    }
}
