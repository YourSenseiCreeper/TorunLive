using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner.Interfaces;
using TorunLive.SIPTimetableScanner.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
//builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddSingleton<IRequestService, RequestService>();
builder.Services.AddSingleton<ITimetableParserService, TimetableParserService>();
builder.Services.AddSingleton<IDelayService, DelayService>();
builder.Services.AddScoped<ILineDirectionsService, LineDirectionsService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<ITimetableScannerService, TimetableScannerService>();
builder.Services.AddDbContext<TorunLiveContext>(options =>
    options.UseSqlServer("Server=LAPTOP-IJH0V32L\\SQLEXPRESS;Database=TorunLive;Trusted_Connection=True;Encrypt=False")
);

using IHost host = builder.Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

var service = provider.GetRequiredService<ITimetableScannerService>();
service.ScanTimetablesAndLines().GetAwaiter().GetResult();