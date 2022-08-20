using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQ.Domain.Database;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue.Models;
using MQ.Domain.Rabbit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MQ.Domain.Queue.Services
{
    public sealed class FailedItemsDbService : IFailedItemsService
    {
        private readonly KernelDbContext _dbCtx;

        public FailedItemsDbService(KernelDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        public void Dispose()
        {
            _dbCtx.Dispose();
        }

        public void ReturnFailedItemToQueue(int failedItemId, IModel model, string queueName)
        {
            var failedItem = _dbCtx.FailedItems.Find(failedItemId);
            string? serializedItem = failedItem?.SerializedItem;
            if (serializedItem is null)
                throw new ArgumentException($"Item with id {failedItemId} was not found", nameof(failedItemId));

            var message = JsonSerializer.Deserialize<DocForDataroomQueueItem>(serializedItem);
            if (message is null)
                throw new ApplicationException($"Cannot deserialize item with id {failedItemId}");

            model.SendDocumentToQueue(queueName, message.ToByteArray());

            _dbCtx.FailedItems.Remove(failedItem!);
            _dbCtx.SaveChanges();
        }

        public void SaveFailedItem(DocForDataroomQueueItem queueItem, Exception itemException)
        {
            queueItem.TriesCount = 0;

            _dbCtx.FailedItems.Add(new FailedItem() 
            { 
                CreationDate = DateTime.Now, 
                ExceptionMessage = itemException.Message, 
                ItemType = ItemTypes.DocImport, 
                SerializedItem = JsonSerializer.Serialize(queueItem) 
            });
        }
    }
}
