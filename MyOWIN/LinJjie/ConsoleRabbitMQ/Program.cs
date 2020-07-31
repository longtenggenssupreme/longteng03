using RabbitMQ.Client;
using System;

namespace ConsoleRabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RabbitMQ 生产者开始。。。生产。。。!");

            #region RabbitMQ 生产者
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            using (var connection = connectionFactory.CreateConnection())
            {
                using var channel = connection.CreateModel();
                channel.QueueDeclare("myqueue", true, false, false, null);
                channel.ExchangeDeclare("myexchange", ExchangeType.Direct, true, false, null);
                channel.QueueBind("myqueue", "myexchange", "myexchangekey", null);
                for (int i = 0; i < 100; i++)
                {
                    var body = System.Text.Encoding.UTF8.GetBytes($"这是发布的数据。{i}。");
                    channel.BasicPublish("myexchange", "myexchangekey", null, body);
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

            #endregion
            Console.WriteLine("RabbitMQ 输入任何字符退出。。");
            Console.Read();
        }
    }
}
