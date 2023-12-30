using Microsoft.AspNetCore.Builder;
using U_ProxMicrosoftEntraIDConnector.Data;
using U_ProxMicrosoftEntraIDConnector.Data.Abstractions;
using U_ProxMicrosoftEntraIDConnector.Data.Repositories;
using U_ProxMicrosoftEntraIDConnector.Services;
using U_ProxMicrosoftEntraIDConnector.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddSingleton<IEntraService, EntraService>();
builder.Services.AddSingleton<IBrockerService, ArtemisService>();

//builder.Services.AddHostedService<HostedService>();

//var hosted = new HostedService();
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