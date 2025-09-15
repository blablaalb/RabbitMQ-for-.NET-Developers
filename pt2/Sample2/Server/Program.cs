using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    const string HostName = "192.168.163.131";
    const string UserName = "admin";
    const string Password = "password";
    private const string QueueName = "Module3.Sample6";
    private const string ExchangeName = "";
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
            Console.WriteLine("Received message");

            //Get Output File Path
            var pathProperty = (byte[])ea.BasicProperties.Headers["OutputFileName"];
            var outputPath = Encoding.Default.GetString(pathProperty);
            var sequenceNumber = (int)ea.BasicProperties.Headers["SequenceNumber"];


            //Adding message                
            using (var fileStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write))
            {
                fileStream.Write(ea.Body.ToArray(), 0, ea.Body.Length);
                fileStream.Flush();
            }
            Console.WriteLine("Message saved to disk - Sequence No = {0}", sequenceNumber);

            await channel.BasicAckAsync(ea.DeliveryTag, false);
            Console.WriteLine("Listening for another message");
        };

        Console.ReadLine();

    }
}