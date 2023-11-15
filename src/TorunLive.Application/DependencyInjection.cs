using Microsoft.Extensions.DependencyInjection;
using TorunLive.Application.Adapters;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Services;

namespace TorunLive.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ILiveRequestService, LiveRequestService>();
            services.AddScoped<IFullTimetableService, FullTimetableService>();
            services.AddScoped<ILiveTimetableAdapter, LiveTimetableAdapter>();
            services.AddScoped<ITimetableAdapter, TimetableAdapter>();
            services.AddScoped<ITimetableComparatorService, TimetableComparatorService>();
            //services.AddScoped<ILineStopsService, LineStopsService>();
            //services.AddSingleton<ILineStopsRepository, LineStopsRepository>();
            return services;
        }
    }
}
