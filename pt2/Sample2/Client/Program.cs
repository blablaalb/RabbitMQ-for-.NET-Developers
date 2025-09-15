using RabbitMQ.Client;
using System.Collections.Specialized;

namespace Client;

class Program
{
    const string HostName = "192.168.163.131";
    const string UserName = "admin";
    const string Password = "password";
    private const string QueueName = "Module3.Sample6";
    private const string ExchangeName = "";
    const string VirtualHost = "dot-net-course";

    private const string InputFile = "BigFile.txt";
    private const int ChunkSize = 4096;


    private static async Task Main(string[] args)
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

        Console.WriteLine("Press enter key to send a message");
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
                break;

            if (key.Key == ConsoleKey.Enter)
            {
                var outputFileName = string.Format("{0}.txt", Guid.NewGuid().ToString());

                var fileStream = File.OpenRead(InputFile);
                var streamReader = new StreamReader(fileStream);
                int remaining = (int)fileStream.Length;
                int length = (int)fileStream.Length;
                var messageCount = 0;

                while (true)
                {
                    if (remaining <= 0)
                        break;

                    //Read Chunk                        
                    int read = 0;
                    byte[] buffer;
                    if (remaining > ChunkSize)
                    {
                        buffer = new byte[ChunkSize];
                        read = fileStream.Read(buffer, 0, ChunkSize);
                    }
                    else
                    {
                        buffer = new byte[remaining];
                        read = fileStream.Read(buffer, 0, remaining);
                    }


                    //Setup properties
                    var properties = new BasicProperties();
                    properties.Persistent = true;
                    properties.Headers = new Dictionary<string, object?>();
                    properties.Headers.Add("OutputFileName", outputFileName);
                    properties.Headers.Add("SequenceNumber", messageCount);

                    //Send message
                    Console.WriteLine("Sending chunk message - Index = {0}; Length = {0}", messageCount, read);
                    await channel.BasicPublishAsync(ExchangeName, QueueName, mandatory: false, properties, buffer);

                    messageCount++;
                    remaining = remaining - read;
                }

                Console.WriteLine("Completed sending {0} chunks", messageCount);
            }
        }

        Console.ReadLine();
    }
}