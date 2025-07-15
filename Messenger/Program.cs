using Messenger.Extensions;
using Messenger.Filters;
using Messenger.Models;
using Messenger.Repositories;
using Messenger.Repositories.Interface;
using Messenger.Services;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Logging

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Platform");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});
LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs\\NeoDaoBackend-.log", rollingInterval: RollingInterval.Day);

if (builder.Environment.IsDevelopment() && false)
{
    loggerConfiguration.MinimumLevel.Debug();
}
else
{
    loggerConfiguration.MinimumLevel.Information();
}

var logger = loggerConfiguration.CreateLogger();
builder.Host.UseSerilog(logger);

#endregion

builder.Services.AddDbContext<MessengerDbContext>();

builder.Services
        .AddScoped<IDirectChatsRepository, DirectChatsRepository>()
        .AddScoped<IGroupChatsRepository, GroupChatsRepository>();

builder.Services
    .AddScoped<DirectChatsService>()
    .AddScoped<GroupChatsService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();


app.MigrateDatabase<MessengerDbContext>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger.API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapGet("/api/public/ping", () => "O Captain! My Captain!");

app.Run();
