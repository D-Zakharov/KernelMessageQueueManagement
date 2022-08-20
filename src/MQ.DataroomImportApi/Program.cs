using System.Diagnostics.Metrics;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MQ.DataroomImportApi.Config;
using MQ.DataroomImportApi.Endpoints;
using MQ.DataroomImportApi.Exceptions;
using MQ.DataroomImportApi.Models;
using MQ.DataroomImportApi.Services;
using MQ.Domain.Database;
using MQ.Domain.Rabbit;
using Serilog;
using static System.Net.Mime.MediaTypeNames;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(loggerConfig);

builder.Services.Configure<RabbitConfiguration>(builder.Configuration.GetSection(nameof(RabbitConfiguration)));
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection(nameof(AppConfiguration)));

builder.Services.AddDbContextFactory<KernelDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetValue<string>("ConnectionString"),
        serverDbContextOptionsBuilder =>
        {
            serverDbContextOptionsBuilder.EnableRetryOnFailure();
        });
});

builder.Services.AddSingleton<QueueConnectionManager>();
builder.Services.AddScoped<IQueueImportService, QueueImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterExceptionHandler();
app.MapDocImportEndpoints();

app.Run();
