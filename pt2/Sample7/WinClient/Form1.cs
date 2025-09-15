using RabbitMQ.Client;
using System.Text;

namespace WinClient;

public partial class Form1 : Form
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string HoldingQueueName = "Module3.Sample11.HoldingQueue";
    private const string VirtualHost = "dot-net-course";

    private int messageCount;

    public Form1()
    {
        InitializeComponent();
    }

    private async void sendButton_Click(object sender, EventArgs e)
    {
        #region Connect to RabbitMQ
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

        //Setup properties
        var properties = new BasicProperties();
        properties.Persistent = true;
        properties.Expiration = "5000";

        //Serialize
        byte[] messageBuffer = Encoding.Default.GetBytes(messageCount.ToString());

        //Send message
        await channel.BasicPublishAsync("", HoldingQueueName, mandatory: false, properties, messageBuffer);

        MessageBox.Show(string.Format("Sending Message - Message - {0}", messageCount.ToString()), "Message sent");

        messageCount++;
    }

    private static string GetComboItem(ComboBox comboBox)
    {
        if (string.IsNullOrEmpty(comboBox.Text))
            return string.Empty;
        return comboBox.Text;
    }
}