using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace MQ.Domain.Rabbit;

public class RabbitConfiguration
{
    public string? Host { get; set; }
    public string? DocsQueueName { get; set; }
    public string? Username { get; set; }
    public string? Pass { get; set; }

    public void CheckBasicProperties()
    {
        foreach (var prop in GetType().GetProperties())
        {
            string? value = (string?)prop.GetValue(this);
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(prop.Name, "Config property is empty");
        }
    }

    public IConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory()
        {
            HostName = Host,
            UserName = Username,
            Password = Pass
        };
    }
}
