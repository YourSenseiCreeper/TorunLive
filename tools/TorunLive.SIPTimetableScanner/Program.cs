using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner;
using TorunLive.SIPTimetableScanner.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Services;
using TorunLive.SIPTimetableScanner.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddSingleton<IRequestService, RequestService>();
builder.Services.AddSingleton<ITimetableAdapterService, TimetableAdapterService>();
builder.Services.AddSingleton<ILineDirectionsAdapterService, LineDirectionsAdapterService>();
builder.Services.AddSingleton<IDelayService, DelayService>();
builder.Services.AddScoped<ILineDirectionsService, LineDirectionsService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<ITimetableScannerService, TimetableScannerService>();
builder.Services.AddDbContext<TorunLiveContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConfigurationKeys.ConnectionString))
);

using IHost host = builder.Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;

string[] linesToScan =
[
    //"1", "2", "3", "4", "5", "6", "7",
    //"10", "11", "12", "13", "14", "15", "16", "17",
    //"18", "19", "20", "24", "25", "26", 
    "27", 
    //"28", "29", "30",
    "31",
    //"32", "33", "34",
    "38",
    //"39",
    //"40", "41", "42", "44", "111", "112", "113", "115",
    //"121", "122", "131", "N90", "N91", "N93", "N94", "N95"
];

var service = provider.GetRequiredService<ITimetableScannerService>();
service.ScanTimetablesAndLines(linesToScan).GetAwaiter().GetResult();