using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections;
using System.Text;

class Program
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string DeadLetterQueueName = "Module3.Sample9.DeadLetter";
    private const string VirtualHost = "dot-net-course";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting RabbitMQ queue processor");
        Console.WriteLine();
        Console.WriteLine();
        DisplaySettings();

        var connectionFactory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost
        };

        var connection = await connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.BasicQosAsync(0, 1, false);
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.BasicConsumeAsync(DeadLetterQueueName, false, consumer);

        consumer.ReceivedAsync += async (ch, ea) =>
        {
            //Serialize message
            var message = Encoding.Default.GetString(ea.Body.ToArray());

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Message Recieved - {0}", message);

            //Change message
            Console.WriteLine("Changing message to be 1");
            message = "1";

            //Resend message                
            var properties = new BasicProperties();
            properties.Persistent = true;
            byte[] messageBuffer = Encoding.Default.GetBytes(message);

            var resubmitQueue = GetQueue(ea);
            await channel.BasicPublishAsync("", resubmitQueue, mandatory: false, properties, messageBuffer);

            //Ack message from Dead Letter Queue
            await channel.BasicAckAsync(ea.DeliveryTag, false);

        };

        Console.ReadLine();
    }

    private static string GetQueue(BasicDeliverEventArgs deliveryArgs)
    {
        if (deliveryArgs.BasicProperties.Headers == null)
            return string.Empty;

        var header = deliveryArgs.BasicProperties.Headers["x-death"] as IList;
        if (header == null || header.Count <= 0)
            return string.Empty;

        var xDeathHeader = header[0] as IDictionary<string, object?>;
        if (xDeathHeader == null || xDeathHeader.Count < 1)
            return string.Empty;

        if (xDeathHeader.ContainsKey("queue"))
        {
            var queueBytes = xDeathHeader["queue"] as byte[];
            return Encoding.Default.GetString(queueBytes);
        }

        return string.Empty;
    }

    /// <summary>
    /// Displays the rabbit settings
    /// </summary>
    private static void DisplaySettings()
    {
        Console.WriteLine("Host: {0}", HostName);
        Console.WriteLine("Username: {0}", UserName);
        Console.WriteLine("Password: {0}", Password);
        Console.WriteLine("Dead Letter QueueName: {0}", DeadLetterQueueName);
    }
}