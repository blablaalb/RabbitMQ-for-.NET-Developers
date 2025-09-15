using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
    private const string QueueName = "Module2.Sample7.Queue";
    private const bool IsDurable = false;
    //The two below settings are just to illustrate how they can be used but we are not using them in
    //this sample as we will use the defaults
    private const string VirtualHost = "dot-net-course";
    private int Port = 0;

    private string _responseQueue;
    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;

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

        //Create dynamic response queue
        var resQueue = await _channel.QueueDeclareAsync();
        _responseQueue = resQueue.QueueName;
        _consumer = new AsyncEventingBasicConsumer(_channel);
        await _channel.BasicConsumeAsync(_responseQueue, true, _consumer);
    }

    public async Task SendAsync(string message, TimeSpan timeout, Action<string> responseCallback)
    {
        var correlationToken = Guid.NewGuid().ToString();

        //Setup properties
        var properties = new BasicProperties();
        properties.ReplyTo = _responseQueue;
        properties.CorrelationId = correlationToken;

        //Serialize
        byte[] messageBuffer = Encoding.Default.GetBytes(message);

        //Send
        await _channel.BasicPublishAsync("", QueueName, mandatory: true, properties, messageBuffer);

        _consumer.ReceivedAsync += async (ch, ea) =>
        {
            if (ea.BasicProperties != null && ea.BasicProperties.CorrelationId == correlationToken)
            {
                var response = Encoding.Default.GetString(ea.Body.ToArray());
                responseCallback(response);
            }
        };
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