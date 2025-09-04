using Server;

Console.WriteLine("Starting RabbitMQ queue processor");
Console.WriteLine();
Console.WriteLine();

var queueProcessor = new RabbitConsumer() { Enabled = true };
await queueProcessor.StartAwait();
Console.ReadLine();