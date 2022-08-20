using MQ.Domain.Database;
using MQ.Domain.Rabbit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MQ.DocImportService.Services;
using MQ.DocImportService.Models.Config;
using MQ.DocImportService;
using MQ.Domain.Queue.Services;
using MQ.DocImportService.Services.Factories;
using MQ.Domain.Queue.Services.Factories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        
        services.Configure<RabbitConfiguration>(configuration.GetSection(nameof(RabbitConfiguration)));
        services.Configure<AppConfiguration>(configuration.GetSection(nameof(AppConfiguration)));
        services.Configure<SharepointConfiguration>(configuration.GetSection(nameof(SharepointConfiguration)));

        services.AddDbContextFactory<KernelDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetValue<string>("ConnectionString"),
                serverDbContextOptionsBuilder =>
                {
                    serverDbContextOptionsBuilder.EnableRetryOnFailure();
                });
        });

        services.AddSingleton<IFailedItemsServiceFactory, FailedItemsDbServiceFactory>();
        services.AddSingleton<IDocStorageConnectionFactory, DataroomConnectionFactory>();
        services.AddSingleton<IDocImportServiceFactory, DataroomImportServiceFactory>();

        services.AddHostedService<Worker>();
    })
    .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))
    .Build();

await host.RunAsync();
