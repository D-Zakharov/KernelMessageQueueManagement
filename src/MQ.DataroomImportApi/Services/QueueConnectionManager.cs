using Microsoft.Extensions.Options;
using MQ.Domain.Rabbit;
using RabbitMQ.Client;

namespace MQ.DataroomImportApi.Services
{
    public sealed class QueueConnectionManager : IDisposable
    {
        private readonly RabbitConfiguration _rabbitConfiguration;

        public IConnectionFactory RabbitConnectionFactory { get; init; }
        public IConnection RabbitConnection { get; init; }

        public QueueConnectionManager (IOptions<RabbitConfiguration> options)
        {
            _rabbitConfiguration = options.Value;
            RabbitConnectionFactory = _rabbitConfiguration.CreateConnectionFactory();
            RabbitConnection = RabbitConnectionFactory.CreateConnection();
        }

        public void Dispose()
        {
            if (RabbitConnection != null)
                RabbitConnection.Dispose();
        }

        public IModel CreateModel()
        {
            return RabbitConnection.CreateModel();
        }
    }
}
