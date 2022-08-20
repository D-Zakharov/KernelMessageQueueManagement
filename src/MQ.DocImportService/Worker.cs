using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using MQ.Domain.Rabbit;
using System.Reflection;
using MQ.DocImportService.Models.Config;
using MQ.DocImportService.Services;
using MQ.Domain.Queue.Models;
using MQ.Domain.Queue.Services;
using MQ.Domain.Queue.Services.Factories;
using MQ.DocImportService.Services.Factories;

namespace MQ.DocImportService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitConfiguration _rabbitConfig;
    private readonly AppConfiguration _appOptions;
    private readonly IDocImportServiceFactory _importServiceFactory;
    private readonly IFailedItemsServiceFactory _failedItemsServiceFactory;

    public Worker(ILogger<Worker> logger, IOptions<RabbitConfiguration> rabbitOptions, IOptions<AppConfiguration> appOptions,
        IDocImportServiceFactory importServiceFactory, IFailedItemsServiceFactory failedItemsServiceFactory)
    {
        (_logger, _rabbitConfig, _appOptions, _importServiceFactory, _failedItemsServiceFactory)
            = (logger, rabbitOptions.Value, appOptions.Value, importServiceFactory, failedItemsServiceFactory);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitConfig.CheckBasicProperties();

        _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

        var factory = _rabbitConfig.CreateConnectionFactory();
        using (var connection = factory.CreateConnection())
        using (var model = connection.CreateModel())
        {
            model.DeclareDocImportQueue(_rabbitConfig);

            var consumer = new AsyncEventingBasicConsumer(model);
            consumer.Received += ExecTask;

            model.BasicConsume(queue: _rabbitConfig.DocsQueueName,
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
            }
        }
    }

    private async Task ExecTask(object consumerModel, BasicDeliverEventArgs ea)
    {
        var consumer = (AsyncEventingBasicConsumer?)consumerModel;

        DocForDataroomQueueItem? message = null;
        try
        {
            var body = ea.Body.ToArray();
            message = DocForDataroomQueueItem.ParseQueueItem(body);

            _logger.LogDebug("Received document num: {message}, date: {date}, type: {type}",
                message?.DocNum, message?.DocDate, message?.DocType);

            if (message is not null)
            {
                using (IDocImportService importService = _importServiceFactory.GetService())
                {
                    var importResult = await importService.ImportDocument(message);
                    await importService.SaveImportedDocumentInfo(message, importResult);
                    await importService.ClearTempFiles(message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            if (message is not null)
            {
                SendBackToQueue(consumer, message, ex);
            }
        }
    }

    private static string SerializeForLog(DocForDataroomQueueItem message)
    {
        return JsonSerializer.Serialize(message);
    }

    private void SendBackToQueue(AsyncEventingBasicConsumer? consumer, DocForDataroomQueueItem message, Exception itemException)
    {
        try
        {
            string messageForLog = SerializeForLog(message);
            _logger.LogError("Data that caused an error: {data}", messageForLog);

            message.TriesCount++;
            if (message.TriesCount < _appOptions.MaxTriesCount)
            {
                if (consumer is not null)
                {
                    message.TriesCount++;
                    consumer.Model.SendDocumentToQueue(_rabbitConfig.DocsQueueName!, message.ToByteArray());
                }
            }
            else
            {
                _logger.LogError("Too many tries, save failed item");
                if (consumer is not null && message is not null)
                {
                    using (var failedItemsService = _failedItemsServiceFactory.GetService())
                    {
                        failedItemsService.SaveFailedItem(message, itemException);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not send a message back to a queue: {text}", ex.Message);
        }
    }
}