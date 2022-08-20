using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQ.DataroomImportApi.Config;
using MQ.DataroomImportApi.Exceptions;
using MQ.DataroomImportApi.Models;
using MQ.Domain.Database;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue;
using MQ.Domain.Rabbit;
using RabbitMQ.Client;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MQ.DataroomImportApi.Services;

public class QueueImportService : IQueueImportService
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly QueueConnectionManager _rabbitConnectionManager;
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly AppConfiguration _appOptions;
    private readonly KernelDbContext _dbCtx;

    public QueueImportService(ILoggerFactory loggerFactory, QueueConnectionManager rabbitConnectionManager, KernelDbContext dbCtx,
        IOptions<RabbitConfiguration> rabbitOptions, IOptions<AppConfiguration> appOptions)
        =>
        (_loggerFactory, _rabbitConnectionManager, _rabbitConfiguration, _appOptions, _dbCtx) =
        (loggerFactory, rabbitConnectionManager, rabbitOptions.Value, appOptions.Value, dbCtx);

    public async Task<IResult> SendDocumentToQueue(ImportRequestModel docData)
    {
        if (docData.Content.Length > _appOptions.MaxFileSizeInBytes)
        {
            throw new ApiException(ErrorCodes.FileIsTooBig);
        }

        bool isKeyExists = await IsIdempotencyKeyInsertSuccessfull(docData.IdempotencyKey);
        if (isKeyExists)
        {
            try
            {
                using (var model = _rabbitConnectionManager.CreateModel())
                {
                    model.DeclareDocImportQueue(_rabbitConfiguration);

                    string filePath = $"{_appOptions.TempFolderForContent}{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
                    using (var binaryReader = new BinaryReader(docData.Content.OpenReadStream()))
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        binaryReader.BaseStream.CopyTo(fileStream);

                        var queueItem = docData.ToQueueItem(filePath);

                        await Task.Run(() => model.SendDocumentToQueue(_rabbitConfiguration.DocsQueueName!, queueItem.ToByteArray()));
                    }

                    var logger = _loggerFactory.CreateLogger("global");
                    logger.Log(LogLevel.Information, "Sent an item to a queue: DocType: {DocType}, DocNum: {DocNum}, DocDate: {DocDate}, MainDocNum: {MainDocNum}, MainDocDate: {MainDocDate}",
                        docData.DocType, docData.DocNum, docData.DocDate, docData.MainDocNum, docData.MainDocDate);
                }
            }
            catch (Exception)
            {
                await RemoveIdempotencyKey(docData.IdempotencyKey);
                throw;
            }
        }

        return Results.Ok();
    }

    private async Task<bool> IsIdempotencyKeyInsertSuccessfull(Guid idempotencyKey)
    {
        try
        {
            _dbCtx.IdempotencyKeys.Add(new IdempotencyKey()
            {
                CreationDate = DateTime.Now,
                Id = idempotencyKey,
                ProjectCode = IdempotencyProjectCodes.ShipmentActs
            });
            await _dbCtx.SaveChangesAsync();

            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    private async Task RemoveIdempotencyKey(Guid idempotencyKey)
    {
        var item = _dbCtx.IdempotencyKeys.Find(idempotencyKey);
        if (item is not null)
        {
            _dbCtx.IdempotencyKeys.Remove(item);
            await _dbCtx.SaveChangesAsync();
        }
    }
}