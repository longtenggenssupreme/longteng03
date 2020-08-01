using RabbitMQ.Client;
using System;
using RabbitMQ.Client.Events;

namespace ConsoleRabbitMQ02
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
            using (var connection = connectionFactory.CreateConnection())
            {
                using var channel = connection.CreateModel();
                // durable: true 队列持久化
                channel.QueueDeclare(queue: "myqueue", durable: true, false, false, null);
                //durable: true 交换机持久化
                channel.ExchangeDeclare(exchange: "myexchange", ExchangeType.Direct, durable: true, false, null);
                channel.QueueBind(queue: "myqueue", exchange: "myexchange", routingKey: "myexchangekey", null);
                var consumer = new EventingBasicConsumer(channel);//消费事件
                consumer.Received += (sender, e) =>
                {

                    //下面操作包括事务处理
                    var body = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                    //
                    //处理消息具体处理过程
                    Console.WriteLine("RabbitMQ 消费者已经消费消息");
                    //

                    ////手动确认，正常消费，通知消息中心，该条消息可以删除了，手动确认的话，自动确认要设置为false，autoAck: true,
                    //channel.BasicAck(e.DeliveryTag, false);
                    //channel.BasicConsume(queue: "myqueue", autoAck: false, consumer);

                    ////手动确认，非正常消费即出错出现异常，通知消息中心，手动确认的话，自动确认要设置为false，autoAck: true,
                    //BasicReject 中requeue: true 告诉消息队列，出错，但是重新把消息插入到队列中，下次使用
                    //BasicReject 中requeue: false 告诉消息队列，出错，删除该条消息
                    channel.BasicReject(e.DeliveryTag, requeue: true);
                    //channel.BasicConsume(queue: "myqueue", autoAck: false, consumer);

                    //autoAck: true,自动确认，表示已成功从消息队列中读取消息，通知消息队列
                    channel.BasicConsume(queue: "myqueue", autoAck: true, consumer);
                };
            }
            #endregion
            Console.WriteLine("RabbitMQ 输入任何字符退出。。");
            Console.Read();
        }
    }
}
