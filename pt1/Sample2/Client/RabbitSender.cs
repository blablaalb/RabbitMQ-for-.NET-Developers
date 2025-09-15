using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client;
public class RabbitSender : IDisposable
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module2.Sample2";
    private const string ExchangeName = "";
    private const bool IsDurable = true;
    //The two below settings are just to illustrate how they can be used but we are not using them in
    //this sample as we will use the defaults
    private const string VirtualHost = "dot-net-course";
    private int Port = 0;

    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IChannel _channel;

    /// <summary>
    /// Ctor
    /// </summary>
    public RabbitSender()
    {
        DisplaySettings();
        SetupRabbitMq().GetAwaiter().GetResult();
    }

    private void DisplaySettings()
    {
        Console.WriteLine("Host: {0}", HostName);
        Console.WriteLine("Username: {0}", UserName);
        Console.WriteLine("Password: {0}", Password);
        Console.WriteLine("QueueName: {0}", QueueName);
        Console.WriteLine("ExchangeName: {0}", ExchangeName);
        Console.WriteLine("VirtualHost: {0}", VirtualHost);
        Console.WriteLine("Port: {0}", Port);
        Console.WriteLine("Is Durable: {0}", IsDurable);
    }
    /// <summary>
    /// Sets up the connections for rabbitMQ
    /// </summary>
    private async Task SetupRabbitMq()
    {
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

        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public async Task SendAsync(string message)
    {
        //Setup properties
        var properties = new BasicProperties();
        properties.Persistent = true;

        //Serialize
        byte[] messageBuffer = Encoding.Default.GetBytes(message);

        //Send message
        await _channel.BasicPublishAsync(ExchangeName, QueueName, mandatory: true, properties, messageBuffer);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        if (_connection != null)
            _connection.CloseAsync().GetAwaiter().GetResult();

        if (_channel != null && _channel.IsOpen)
            _channel.AbortAsync().GetAwaiter().GetResult();

        _connectionFactory = null;

        GC.SuppressFinalize(this);
    }
}