using DotNetAzure_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAzure_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisCacheController : ControllerBase
    {
        private readonly ITaskService _service;

        public RedisCacheController(ITaskService service)
        {
            _service = service;
        }
        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetCachedTask(Guid taskId)
        {
            var myTasks = await _service.GetTaskAsync(taskId);
            return Ok(myTasks);
        }
    }
}
