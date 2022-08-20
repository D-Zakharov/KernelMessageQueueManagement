using MQ.Domain.Queue;
using MQ.Domain.Rabbit;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using System;
using MQ.Domain.Queue.Models;

while (true)
{
    byte[] fileData = File.ReadAllBytes(@"C:\Users\Zak\Downloads\script.txt");

    
    var docData = new DocForDataroomQueueItem()
    {
        DocDate = DateTime.Now,
        DocNum = $"test {DateTime.Now:HH:mm:ss}", 
        DocType = "Договор",
        ContentPath = @$"C:\Temp\{Guid.NewGuid}"
    };
    File.WriteAllText(docData.ContentPath, "test conentnt");
    var body = docData.ToByteArray();

    var config = new RabbitConfiguration()
    {
        Host = "localhost",
        DocsQueueName = "dataroom-import",
        Pass = "guest",
        Username = "guest"
    };

    var rabbitConnectionFactory = config.CreateConnectionFactory();

    using (var connection = rabbitConnectionFactory.CreateConnection())
    {
        var model = connection.CreateModel();
        model.SendDocumentToQueue(config.DocsQueueName, body);
    }

    Console.WriteLine("message was sent");
    Console.ReadLine();
}
