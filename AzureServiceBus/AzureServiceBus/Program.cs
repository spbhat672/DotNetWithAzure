// See https://aka.ms/new-console-template for more information
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", optional: false)
    .Build();

string namespaceName =

    config["ServiceBus:FullyQualifiedNamespace"]

    ?? throw new InvalidOperationException("Namespace is missing.");

string queueName =

    config["ServiceBus:QueueName"]

    ?? throw new InvalidOperationException("Queue name is missing.");

await using var client = new ServiceBusClient(

    namespaceName,

    new DefaultAzureCredential(),

    new ServiceBusClientOptions
    {

        TransportType = ServiceBusTransportType.AmqpWebSockets

    });

ServiceBusSender sender = client.CreateSender(queueName);

var message = new ServiceBusMessage("Hello from my .NET 8 application!");

await sender.SendMessageAsync(message);

Console.WriteLine("Message sent successfully!");

// Receive the message
ServiceBusReceiver receiver = client.CreateReceiver(queueName);

ServiceBusReceivedMessage? receivedMessage =

    await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(10));

if (receivedMessage is not null)

{

    Console.WriteLine($"Received: {receivedMessage.Body}");


    // Remove the message after successful processing
    await receiver.CompleteMessageAsync(receivedMessage);

}
else
{

    Console.WriteLine("No message received.");

}