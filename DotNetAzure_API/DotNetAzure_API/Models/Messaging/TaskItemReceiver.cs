using Azure.Messaging.ServiceBus;
using DotNetAzure_API.Services;
using System.Text.Json;

namespace DotNetAzure_API.Models.Messaging
{
    public class TaskItemReceiver : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly TaskApiClient _taskApiClient;
        private readonly ServiceBusProcessor _processor;
        public TaskItemReceiver(

    ServiceBusClient client,

    TaskQueueSetting settings,

    ILogger<TaskItemReceiver> logger, TaskApiClient taskApiClient)

        {

            _logger = logger;

            _taskApiClient = taskApiClient;


            _processor = client.CreateProcessor(

                settings.QueueName,

                new ServiceBusProcessorOptions
                {

                    AutoCompleteMessages = false,

                    MaxConcurrentCalls = 1

                });


            _processor.ProcessMessageAsync += HandleMessageAsync;
            _processor.ProcessErrorAsync += HandleErrorAsync;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)

        {
            _logger.LogInformation("Task Item message worker started.");

            try
            {
                await _processor.StartProcessingAsync(stoppingToken);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
            }
        }

        private Task HandleErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Service Bus error. ErrorSource: {ErrorSource}, EntityPath: {EntityPath}",
                args.ErrorSource,
                args.EntityPath);

            return Task.CompletedTask;
        }


        private async Task HandleMessageAsync(
    ProcessMessageEventArgs args)
        {
            try
            {
                var item = args.Message.Body
                    .ToObjectFromJson<TaskItemCreatedEvent>();

                if (item is null)
                {
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        "InvalidOrder",
                        "Order message is empty.");

                    return;
                }

                await _taskApiClient.CreateTaskAsync(item, args.CancellationToken);

                _logger.LogInformation(
                    "Processing item {Title} for {Description}. cDate {CreatedOnUtc}, uDate: {UpdatedOnUtc}",
                    item.Title,
                    item.Description,
                    item.CreatedOnUtc,
                    item.UpdatedOnUtc);

                // Simulate your business operation.// For example: reserve inventory or create a notification.await args.CompleteMessageAsync(args.Message);

                _logger.LogInformation(
                    "Item processed successfully.for {Title}",
                    item.Title);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid Item message.");

                await args.DeadLetterMessageAsync(
                    args.Message,
                    "InvalidJson",
                    "Could not deserialize the Item event.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Item processing failed for message {MessageId}.",
                    args.Message.MessageId);

                await args.AbandonMessageAsync(args.Message);
            }
        }

    }
}
