using DotNetAzure_API.Models;
using System.Net.NetworkInformation;

namespace DotNetAzure_API.Services
{
    public class TaskApiClient
    {
            private readonly HttpClient _httpClient;
            private readonly IConfiguration _configuration;
            private readonly ILogger<TaskApiClient> _logger;

            public TaskApiClient(
                HttpClient httpClient,
                IConfiguration configuration,
                ILogger<TaskApiClient> logger)
            {
                _httpClient = httpClient;
                _configuration = configuration;
                _logger = logger;
            }

            public async Task CreateTaskAsync(
                TaskItemCreatedEvent task,
                CancellationToken cancellationToken)
            {
                var endpoint =
                    _configuration["TaskItemApi:CreateTaskEndpoint"]
                    ?? "api/taskItems";

                _logger.LogInformation(
          "Calling Product API: {Url}",
          new Uri(_httpClient.BaseAddress!, endpoint));

                using var response =
                    await _httpClient.PostAsJsonAsync(
                        endpoint,
                        task,
                        cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody =
                        await response.Content.ReadAsStringAsync(
                            cancellationToken);

                    _logger.LogError(
                        "Create Task API failed. Status: {StatusCode}. Response: {Response}",
                        (int)response.StatusCode,
                        responseBody);

                    response.EnsureSuccessStatusCode();
                }

                _logger.LogInformation(
                    "Create Task API called successfully for {Title}",
                    task.Title);
            }
        }

    }
