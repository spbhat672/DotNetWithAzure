using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace DotNetAzure_API.Models.Messaging
{
    public class TaskItemSender
    {
        private readonly ServiceBusSender _sender;
        public TaskItemSender(
        ServiceBusClient client,
        TaskQueueSetting settings)
        {
            _sender = client.CreateSender(settings.QueueName);
        }

        public async Task SendAsync(
    TaskItemCreatedEvent order,
    CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(order);

            var message = new ServiceBusMessage(
                BinaryData.FromString(json))
            {
                ContentType = "application/json",
                Subject = "OrderCreated",
                MessageId = Guid.NewGuid().ToString()
            };

            await _sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
