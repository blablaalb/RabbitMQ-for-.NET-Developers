using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Class to encapsulate recieving messages from RabbitMQ
/// </summary>
public class RabbitConsumer : IDisposable
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module2.Sample3.Queue1";
    private const bool IsDurable = true;
    //The two below settings are just to illustrate how they can be used but we are not using them in
    //this sample as we will use the defaults
    private const string VirtualHost = "dot-net-course";
    private int Port = 0;

    public delegate void OnReceiveMessage(string message);

    public bool Enabled { get; set; }

    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IChannel _channel;

    /// <summary>
    /// Ctor with a key to lookup the configuration
    /// </summary>
    public RabbitConsumer()
    {
        DisplaySettings();
        _connectionFactory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password
        };

        if (string.IsNullOrEmpty(VirtualHost) == false)
            _connectionFactory.VirtualHost = VirtualHost;
        if (Port > 0)
            _connectionFactory.Port = Port;

        _connection = _connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Displays the rabbit settings
    /// </summary>
    private void DisplaySettings()
    {
        Console.WriteLine("Host: {0}", HostName);
        Console.WriteLine("Username: {0}", UserName);
        Console.WriteLine("Password: {0}", Password);
        Console.WriteLine("QueueName: {0}", QueueName);
        Console.WriteLine("VirtualHost: {0}", VirtualHost);
        Console.WriteLine("Port: {0}", Port);
        Console.WriteLine("Is Durable: {0}", IsDurable);
    }
    /// <summary>
    /// Starts receiving a message from a queue
    /// </summary>
    public async Task StartAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        await _channel.BasicConsumeAsync(QueueName, false, consumer);

        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var message = Encoding.Default.GetString(ea.Body.ToArray());
            Console.WriteLine("Message Recieved - {0}", message);
           await  _channel.BasicAckAsync(ea.DeliveryTag, false);
        };
    }

        /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        if (_channel != null)
            _channel.Dispose();
        if (_connection != null)
            _connection.Dispose();

        _connectionFactory = null;

        GC.SuppressFinalize(this);
    }
}
