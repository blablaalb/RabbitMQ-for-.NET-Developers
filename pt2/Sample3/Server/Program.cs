using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Server;

class Program
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module3.Sample7";
    private const string VirtualHost = "dot-net-course";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting RabbitMQ queue processor");
        Console.WriteLine();
        Console.WriteLine();

        #region Connect to RabbitMQ
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
        #endregion

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
                //Discard the message because it cant be processed
                Console.WriteLine("Rejecting the message as it cant be processed - {0}", message);
                await channel.BasicRejectAsync(ea.DeliveryTag, false);
            }
            else
            {
                //Reject the message so it can be retried - EG application error processing message
                Console.WriteLine("Rejecting the message as there was an error processing it - {0}", message);
                await channel.BasicRejectAsync(ea.DeliveryTag, true);
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