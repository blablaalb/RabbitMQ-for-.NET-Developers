using Server;

Console.WriteLine("Starting RabbitMQ queue processor - Server 3");
Console.WriteLine();
Console.WriteLine();

var queueProcessor = new RabbitConsumer() { Enabled = true };
await queueProcessor.StartAsync();
Console.ReadLine();