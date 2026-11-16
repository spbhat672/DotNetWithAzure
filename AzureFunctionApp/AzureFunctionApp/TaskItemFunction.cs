using Azure.Messaging.ServiceBus;
using AzureFunctionApp.Models;
using AzureFunctionApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace AzureFunctionApp
{
    public class TaskItemFunction
    {
        private readonly ILogger<TaskItemFunction> _logger;
        private readonly TaskApiClient _taskApiClient;

        public TaskItemFunction(ILogger<TaskItemFunction> logger, TaskApiClient taskApiClient)
        {
            _logger = logger;
            _taskApiClient = taskApiClient;
        }

        [Function(nameof(TaskItemFunction))]
        public async Task Run([ServiceBusTrigger("demo-queue",Connection ="ServiceBusConnection")] ServiceBusReceivedMessage serviceBusMessage, ServiceBusMessageActions messageActions, CancellationToken cancellationToken)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var serviceMessage = serviceBusMessage.Body;            

            var item = serviceBusMessage.Body.ToObjectFromJson<TaskItemCreatedEvent>();


            if (item is null)

            {
                _logger.LogWarning("Invalid item message. Dead-lettering.");
                await messageActions.DeadLetterMessageAsync(serviceBusMessage,cancellationToken: cancellationToken);
                return;
            }


            await _taskApiClient.CreateTaskAsync(item,cancellationToken);


            _logger.LogInformation(

                "item successfully sent to TaskItem API. Message ID: {MessageId}",serviceBusMessage.MessageId);

            await messageActions.CompleteMessageAsync(serviceBusMessage,cancellationToken);
        }
    }
}
