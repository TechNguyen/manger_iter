using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using It_Supporter.Controllers;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using NuGet.Protocol;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace It_Supporter.Repository
{
    public class NotificationRep : INotiFication
    {
        private readonly ILogger<NotificationRep> _logger;
        private readonly ThanhVienContext _context;
        private readonly ConnectionMultiplexer _redis;
        private readonly Channel<Notification> _channel;

        private readonly IServiceProvider _serviceProvider;

        public NotificationRep(ThanhVienContext context, IServiceProvider serviceProvider, ILogger<NotificationRep> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _context = context;
            _redis = ConnectionMultiplexer.Connect("localhost:6359");
            
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