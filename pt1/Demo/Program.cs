using RabbitMQ;
using RabbitMQ.Client;
using System.Text;

const string HostName = "192.168.163.131";
const string UserName = "admin";
const string Password = "password";
const string QueueName = "Module1.Sample3";
const string ExchangeName = "";
const string VirtualHost = "dot-net-course";

var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
{
    Password = Password,
    UserName = UserName,
    HostName = HostName,
    VirtualHost = VirtualHost
};
var connection = await connectionFactory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();
var msgProperties = new BasicProperties();
msgProperties.Persistent = true;

await channel.QueueDeclareAsync(QueueName, true, false, false, null);
Console.WriteLine("Queue declared.");

//await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic);
//Console.WriteLine("Exchange declared.");

//await channel.QueueBindAsync(QueueName, ExchangeName, "");
//Console.WriteLine("Exchange and queue bound");

byte[] messageBuffer = Encoding.Default.GetBytes("This is my message");

await channel.BasicPublishAsync(ExchangeName, QueueName, mandatory: true, msgProperties, messageBuffer);
Console.WriteLine("Message sent");

Console.ReadLine();
