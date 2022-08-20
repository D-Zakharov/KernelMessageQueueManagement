using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MQ.Domain.Rabbit
{
    public static class ExtensionMethods
    {
        public static void DeclareDocImportQueue(this IModel model, RabbitConfiguration config)
        {
            model.QueueDeclare(queue: config.DocsQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public static void SendDocumentToQueue(this IModel model, string docsQueueName, ReadOnlyMemory<byte> body)
        {
            var properties = model.CreateBasicProperties();
            properties.Persistent = true;

            model.BasicPublish(exchange: "",
                                routingKey: docsQueueName,
                                basicProperties: properties,
                                body: body);
        }
    }
}
