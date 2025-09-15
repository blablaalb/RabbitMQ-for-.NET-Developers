using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Server;

class Program
{
    private const string RetryHeader = "RETRY-COUNT";
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module3.Sample9.Normal";
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
        await channel.BasicConsumeAsync(QueueName, false, consumer);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            //Serialize message
            var message = Encoding.Default.GetString(ea.Body.ToArray());

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Message Recieved - {0}", message);

            if (message == "1")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Acknowledging successful processing of message");
                Console.ForegroundColor = ConsoleColor.White;
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Message is <> 1 so rejecting and not requeuing message");
                Console.ForegroundColor = ConsoleColor.White;

                await channel.BasicRejectAsync(ea.DeliveryTag, false);
            }
        };

        Console.ReadLine();
    }

    /// <summary>
    /// Displays the rabbit settings
    /// </summary>
    private static void DisplaySettings()
    {
        Console.WriteLine("Host: {0}", HostName);
        Console.WriteLine("Username: {0}", UserName);
        Console.WriteLine("Password: {0}", Password);
        Console.WriteLine("QueueName: {0}", QueueName);
    }
}