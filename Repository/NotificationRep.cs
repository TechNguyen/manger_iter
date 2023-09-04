using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using It_Supporter.Controllers;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using NuGet.Protocol;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace It_Supporter.Repository
{
    public class NotificationRep : INotiFication
    {

        private readonly ThanhVienContext _context;
        public NotificationRep(ThanhVienContext context)
        {
            _context = context;
        }
        public async Task<Notification> pubNotification() {
            Notification newNoti = new Notification();
            var factory = new ConnectionFactory() {
                HostName = "localhost",
                 UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();
            var consumer  = new EventingBasicConsumer(channel);
            consumer.Received += (model, es) => {
                var body = es.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                newNoti.NotiBody = "Vua them 1 add";
            };
            channel.BasicConsume(queue: "addFormTech", autoAck: true, consumer: consumer);
            return newNoti;
        }
    }
}