using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Server;

class Program
{
    private const string RetryHeader = "RETRY-COUNT";
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module3.Sample8";
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
            else if (message == "2")
            {
                var attempts = GetRetryAttempts(ea.BasicProperties);
                if (attempts < 3)
                {
                    Console.WriteLine("Message is 2 so rejecting and requeuing message");
                    Console.WriteLine("Attempts made: {0}", attempts);

                    //Create retry message
                    attempts++;
                    var properties = new BasicProperties();
                    properties.Headers = CopyMessageHeaders(ea.BasicProperties.Headers);
                    SetRetryAttempts(properties, attempts);

                    //Publish new updated message for retry
                    await channel.BasicPublishAsync(ea.Exchange, ea.RoutingKey, mandatory:false, properties, ea.Body);

                    //Ack original message
                    await channel.BasicAckAsync(ea.DeliveryTag, false);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Message rejected for retry");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Message is 2 but has has already made {0} attempts so rejecting the message as retries exhausted", attempts);
                    Console.ForegroundColor = ConsoleColor.White;

                    //Reject message all retries used
                    await channel.BasicRejectAsync(ea.DeliveryTag, false);
                }
            }
            else if (message == "3")
            {
                var attempts = GetRetryAttempts(ea.BasicProperties);
                if (attempts < 2)
                {
                    Console.WriteLine("Message is 3 so rejecting and requeuing message");
                    Console.WriteLine("Attempts made: {0}", attempts);

                    attempts++;

                    //Create retry message
                    var properties = new BasicProperties();
                    properties.Headers = CopyMessageHeaders(ea.BasicProperties.Headers);
                    SetRetryAttempts(properties, attempts);

                    //Publish retry message
                    await channel.BasicPublishAsync(ea.Exchange, ea.RoutingKey, mandatory:false, properties, ea.Body);

                    //Ack original message
                    await channel.BasicAckAsync(ea.DeliveryTag, false);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Message rejected for retry");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Message is 3 and {0} attempts have been made so this one will work successfully, and the message is acknowledged", attempts);
                    Console.ForegroundColor = ConsoleColor.White;

                    //Message Processed successfully so ack
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Message is >3 so rejecting and not requeuing message");
                Console.ForegroundColor = ConsoleColor.White;

                //Message scenario always rekected
                await channel.BasicRejectAsync(ea.DeliveryTag, false);
            }
        };

        Console.ReadLine();
    }

    private static IDictionary<string, object?> CopyMessageHeaders(IDictionary<string, object?> existingProperties)
    {
        var newProperties = new Dictionary<string, object?>();
        if (existingProperties != null)
        {
            var enumerator = existingProperties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                newProperties.Add(enumerator.Current.Key, enumerator.Current.Value);
            }
        }
        return newProperties;
    }

    private static void SetRetryAttempts(IBasicProperties properties, int newAttempts)
    {
        if (properties.Headers.ContainsKey(RetryHeader))
            properties.Headers[RetryHeader] = newAttempts;
        else
            properties.Headers.Add(RetryHeader, newAttempts);
    }

    private static int GetRetryAttempts(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers == null || properties.Headers.ContainsKey(RetryHeader) == false)
            return 0;

        var val = properties.Headers[RetryHeader];
        if (val == null)
            return 0;

        return Convert.ToInt32(val);
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