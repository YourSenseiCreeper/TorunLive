using Microsoft.Extensions.DependencyInjection;

namespace TorunLive.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services)
        {
            services.AddDbContext<TorunLiveContext>();
            //services.AddDbContext<TorunLiveContext>(options =>
            //{
            //    options.UseSqlServer();
            //});
            return services;
        }
    }
}
