
using ExpensesService.Api;

var appSettingsConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .Build();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(b =>
    {
        b.UseStartup<Startup>();
    })
    .ConfigureAppConfiguration((_, b) =>
    {
        b.AddConfiguration(appSettingsConfiguration);
    });

var app = builder.Build();

app.Run();

