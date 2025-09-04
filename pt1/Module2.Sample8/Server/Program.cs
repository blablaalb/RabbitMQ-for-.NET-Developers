using Server;

Console.WriteLine("Starting RabbitMQ queue processor - Server 1");
Console.WriteLine();
Console.WriteLine();

var queueProcessor = new RabbitConsumer() { Enabled = true };
await queueProcessor.StartAsync();
Console.ReadLine();