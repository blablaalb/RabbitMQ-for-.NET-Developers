using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server;

class Program
{
    const string HostName = "192.168.163.131";
    const string UserName = "admin";
    const string Password = "password";
    const string QueueName = "Module3.Sample5";
    const string ExchangeName = "";
    const string VirtualHost = "dot-net-course";


    static async Task Main(string[] args)
    {
        #region Connect to Rabbit
        var connectionFactory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost
        };

        var connection = await connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        #endregion

        await channel.BasicQosAsync(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(QueueName, false, consumer);

        Console.WriteLine("Started and listening for a message");
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            //Get message and Deserialize
            var fileName = string.Format("{0}.txt", Guid.NewGuid().ToString());
            File.WriteAllBytes(fileName, ea.Body.ToArray());

            Console.WriteLine("Message saved to disk - {0}", fileName);

            await channel.BasicAckAsync(ea.DeliveryTag, false);
            Console.WriteLine("Listening for another message");
        };
     
        Console.ReadLine();
    }
}