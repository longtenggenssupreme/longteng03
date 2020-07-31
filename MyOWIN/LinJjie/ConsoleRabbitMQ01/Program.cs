﻿using RabbitMQ.Client;
using System;
using RabbitMQ.Client.Events;

namespace ConsoleRabbitMQ01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RabbitMQ 消费者开始。。。消费。。。!");

            #region RabbitMQ 消费者
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            using var connection = connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("myqueue", false, false, false, null);
            channel.ExchangeDeclare("myexchange", null, false, false, null);
            channel.QueueBind("myqueue", "myexchange", "myexchangekey", null);
            var received = new EventingBasicConsumer(channel);
            received.Received += (sender, e) =>
            {
                var body = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                channel.BasicConsume("myqueue", true, received);
            };
            #endregion
            Console.WriteLine("RabbitMQ 输入任何字符退出。。");
            Console.Read();
        }

        private static void Received_Received(object sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}