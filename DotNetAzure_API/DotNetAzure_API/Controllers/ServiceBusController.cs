using DotNetAzure_API.Models;
using DotNetAzure_API.Models.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetAzure_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private readonly TaskItemSender _taskItemSender;

        public ServiceBusController(TaskItemSender taskItemSender) {
         _taskItemSender = taskItemSender;
        }


        [HttpPost]
        public async Task SendSericeBusMessage([FromBody]TaskItemCreatedEvent task, CancellationToken cancellationToken)
        {
            var taskItem = new TaskItemCreatedEvent() { Title= task.Title , Description = task.Description,
                                                        CreatedOnUtc=task.CreatedOnUtc,UpdatedOnUtc=task.UpdatedOnUtc};
            await _taskItemSender.SendAsync(taskItem,cancellationToken);
        }
    }
}
