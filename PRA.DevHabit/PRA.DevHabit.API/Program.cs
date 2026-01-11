using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRA.DevHabit.API.Extensions;
using PRA.DevHabit.Data.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;

})
.AddXmlSerializerFormatters();

builder.Services.AddOpenApi();

#region DI Database

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"),
    sqlServerOptions => sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application)));

#endregion DI Database

#region Telemetry and Logging

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation())
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation())
    .UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
});

#endregion Telemetry and Logging

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Only apply migrations through code-first in development; different approach for production
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
