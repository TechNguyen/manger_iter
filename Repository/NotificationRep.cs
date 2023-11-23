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
using It_Supporter.DataRes;
using Microsoft.AspNetCore.Http.HttpResults;

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
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { { "redis-14916.c1.ap-southeast-1-1.ec2.cloud.redislabs.com", 14916 } },
                User = "default",
                Password = "esEi9Y9Hiuy9GO6O7duIJkqH4mWqTy3t",
                // Ssl = true,
                // SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            };

            _redis = ConnectionMultiplexer.Connect(options);
            
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
            channel.QueueDeclare(queue: "notifiall", durable: true, arguments: null);
            var body = Encoding.UTF8.GetBytes("Admin has add new notif");
            channel.BasicPublish(exchange: string.Empty, routingKey: "notificall", basicProperties: null,body: body);
            return newNoti;
        }
        //insert new notification when add a new post
        public async Task<bool> pushNotiWhenPost(Notification noty)
        {
            try {
                _context.Notification.AddAsync(noty);
                int row = _context.SaveChanges();
                if(row > 0)
                {
                    return true;
                } else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                return false;
            }
        }
        // get all notifycation
        public async Task<ProducerResNotification?> getNoti(string id) 
        {
            try
            {
                ProducerResNotification producer = new ProducerResNotification();
                producer.notifications = new List<Notification>();
                var noties = _context.Notification.Where(e => e.ToUserId == id || e.ToUserId == "1").ToList();
                foreach (var noti in noties)
                {
                    producer.notifications.Add(noti);
                }
                producer.statuscode = 200;
                producer.message = "Get all notification successfullly!";
                return producer;
            } catch (Exception ex)
            {
                return null;
            }
        }
        //delete
        public async Task<bool> deleteNoti(int notiid)
        {
            try
            {
                var rs = _context.Notification.FirstOrDefault(e => e.NotiId == notiid);
                rs.isRead = 1;
                int row = _context.SaveChanges();
                return row > 0 ? true : false;
            } catch (Exception ex)
            {
                return false; 
            }
        }
        //delete notification

    }
}