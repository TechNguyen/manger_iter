using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace It_Supporter.Services
{
    public class messageProducer : ISendingMesage
    {
         public void SendingMessage<T>(T message) {
            var factory = new ConnectionFactory() {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var con = factory.CreateConnection();
            using var channel = con.CreateModel();
            channel.QueueDeclare(queue: "addFormTech", durable: false, exclusive: false);
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);
            channel.BasicPublish(exchange: string.Empty, routingKey: "addFormTech", basicProperties: null, body: body);
         }
    }
}