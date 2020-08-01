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

                // durable: true 队列持久化
                channel.QueueDeclare(queue: "myqueue", durable: true, false, false, null);
                //durable: true 交换机持久化
                channel.ExchangeDeclare(exchange: "myexchange", ExchangeType.Direct, durable: true, false, null);

                //持久化消息，告诉消息队列，该条消息需要持久化和固化到磁盘
                var propertyPersist = channel.CreateBasicProperties();
                propertyPersist.Persistent = true;

                channel.QueueBind(queue: "myqueue", exchange: "myexchange", routingKey: "myexchangekey", null);

                #region Tx事务处理，不推荐使用，处理过程较复杂

                //channel.TxSelect();//开起事务 1

                //for (int i = 0; i < 100; i++)
                //{
                //    var body = System.Text.Encoding.UTF8.GetBytes($"这是发布的数据。{i}。");
                //    //持久化消息 basicProperties: propertyPersist
                //    channel.BasicPublish(exchange: "myexchange", routingKey: "myexchangekey", basicProperties: propertyPersist, body);//发送消息给消息队列，之后消息队列收到以后会进行初持久化处理，存储路径C:\Users\Administrator\AppData下面的RabbitMQ，query文件中
                //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                //}
                //try
                //{
                //    channel.TxCommit();//提交事务 1
                //}
                //catch (Exception ex)
                //{
                //    //这个说明生产者发送消息到消息队列时出错了，这里可以记录错误，也可以重试再次发送等等处理
                //    Console.WriteLine($"RabbitMQ 生产者发送消息到消息队列时出错了，错误信息：{ex.Message}");
                //    channel.TxRollback();//回滚事务 1
                //}
                #endregion

                #region Tx事务处理，推荐使用

                
                try
                {
                    channel.ConfirmSelect();//开起消息确认模式 2 这个rabbitmq的扩展，可以看成一个回调

                    for (int i = 0; i < 100; i++)
                    {
                        var body = System.Text.Encoding.UTF8.GetBytes($"这是发布的数据。{i}。");
                        //持久化消息 basicProperties: propertyPersist
                        channel.BasicPublish(exchange: "myexchange", routingKey: "myexchangekey", basicProperties: propertyPersist, body);//发送消息给消息队列，之后消息队列收到以后会进行初持久化处理，存储路径C:\Users\Administrator\AppData下面的RabbitMQ，query文件中
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

                    //使用下面2中确认方式
                    //第一种
                    if (channel.WaitForConfirms())//返回true 表示消息发送到消息队列，否则发送失败
                    {
                        Console.WriteLine("RabbitMQ 生产者发送消息到消息队成功");
                    }

                    //第二种
                    //channel.WaitForConfirmsOrDie();//确认消息发送到消息队列,发送成功则继续执行，否则即没发成功的话就会报错，抛出异常，在catch中捕获处理
                }
                catch (Exception ex)
                {
                    //这个说明生产者发送消息到消息队列时出错了，这里可以记录错误，也可以重试再次发送等等处理
                    Console.WriteLine($"RabbitMQ 生产者发送消息到消息队列时出错了，错误信息：{ex.Message}");
                    channel.TxRollback();//回滚事务 2
                }
                #endregion

            }

            #endregion
            Console.WriteLine("RabbitMQ 输入任何字符退出。。");
            Console.Read();
        }
    }
}
