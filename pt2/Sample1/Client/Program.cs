using RabbitMQ.Client;

namespace Client;

class Program
{
    const string HostName = "192.168.163.131";
    const string UserName = "admin";
    const string Password = "password";
    const string QueueName = "Module3.Sample5";
    const string ExchangeName = "";
    const string VirtualHost = "dot-net-course";

    private const string InputFile = "BigFile.txt";


    private static async Task Main(string[] args)
    {
        #region Connect to Rabbit
        var connectionFactory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost,
            RequestedFrameMax = 182891520,
            MaxInboundMessageBodySize = 182891520,
            RequestedHeartbeat = TimeSpan.FromSeconds(60)
        };

        var connection = await connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        channel.BasicReturnAsync += async (model, ea) =>
        {
            Console.WriteLine($"❌ Message returned by broker: {ea.ReplyText}");
            Console.WriteLine($"  Reason: {ea.ReplyCode} - {ea.Exchange} / {ea.RoutingKey}");
        };
        #endregion

        var messageCount = 0;

        Console.WriteLine("Press enter key to send a message");
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
                break;

            if (key.Key == ConsoleKey.Enter)
            {
                //Setup properties
                var properties = new BasicProperties();
                properties.Persistent = true;

                //Read File
                Console.WriteLine("Reading file - {0}", InputFile);
                byte[] messageBuffer = File.ReadAllBytes(InputFile).ToArray();
                //messageBuffer = System.Text.Encoding.UTF8.GetBytes("Hello World - " + messageCount).ToArray();
                //Send message
                Console.WriteLine("Sending large message - {0}", messageBuffer.Length);
                await channel.BasicPublishAsync(ExchangeName, QueueName, mandatory: false, properties, messageBuffer);
                messageCount++;
                Console.WriteLine("Message sent");
            }
        }

        Console.ReadLine();
    }
}