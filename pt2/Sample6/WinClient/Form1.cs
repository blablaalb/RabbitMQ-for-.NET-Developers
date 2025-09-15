using RabbitMQ.Client;
using System.Text;

namespace WinClient;

public partial class Form1 : Form
{
    private const string HostName = "192.168.163.131";
    private const string UserName = "admin";
    private const string Password = "password";
    private const string ExchangeName = "Module3.Sample10.Exchange";
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

        var connection =await connectionFactory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        #endregion

        var routingKey = GetComboItem(productComboBox);

        //Setup properties
        var properties = new BasicProperties();
        properties.Persistent =true;

        //Serialize
        byte[] messageBuffer = Encoding.Default.GetBytes(routingKey);

        //Send message
        await channel.BasicPublishAsync(ExchangeName, routingKey, mandatory: false, properties, messageBuffer);

        MessageBox.Show(string.Format("Sending Message - Routing Key - {0}", routingKey), "Message sent");

        messageCount++;
    }

    private static string GetComboItem(ComboBox comboBox)
    {
        if (string.IsNullOrEmpty(comboBox.Text))
            return string.Empty;
        return comboBox.Text;
    }
}