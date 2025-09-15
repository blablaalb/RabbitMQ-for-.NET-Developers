using RabbitMQ.Client;
using System.Text;

namespace ClientUI;

public partial class Form1 : Form
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string QueueName = "Module3.Sample9.Normal";
    private const string VirtualHost = "dot-net-course";

    public Form1()
    {
        InitializeComponent();
    }

    private async void sendButton_Click(object sender, EventArgs e)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost
        };

        var connection = await connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        //Setup properties
        var properties = new BasicProperties();
        properties.Persistent = true;

        //Serialize
        byte[] messageBuffer = Encoding.Default.GetBytes(messageTextBox.Text);

        //Send message
        await channel.BasicPublishAsync("", QueueName, mandatory: false, properties, messageBuffer);
    }
}