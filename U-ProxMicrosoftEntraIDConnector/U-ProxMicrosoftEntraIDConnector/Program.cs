using U_ProxMicrosoftEntraIDConnector.Data;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Repositories;
using U_ProxMicrosoftEntraIDConnector.Services;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

using NLog;
using NLog.Web;
using U_ProxMicrosoftEntraIDConnector.Common;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

StaticConnections.Logger = logger;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var key = builder.Configuration.GetValue<string>("SecureToken");
    if (key != null)
        StaticConnections.SecureToken = key;

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddDbContext<DataContext>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
    builder.Services.AddScoped<IEntraService, EntraService>();
    //builder.Services.AddScoped<IBrockerService, ActiveMQClassicService>();
    builder.Services.AddScoped<IBrockerService, ArtemisService>();

    new Thread(() => new HostedService()).Start();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex);
}
finally
{
    LogManager.Shutdown();
}