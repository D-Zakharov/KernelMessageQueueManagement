using MQ.Domain.Queue.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MQ.Domain.Queue.Services
{
    public interface IFailedItemsService : IDisposable
    {
        public void SaveFailedItem(DocForDataroomQueueItem queueItem, Exception itemException);
        public void ReturnFailedItemToQueue(int failedItemId, IModel model, string queueName);
    }
}