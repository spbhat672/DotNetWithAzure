using DotNetAzure_API.Data;
using DotNetAzure_API.Models;
using DotNetAzure_API.Models.Entities;
using DotNetAzure_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAzure_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskItemsController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;

        }

        [HttpGet]
        public IActionResult GetAllTaskItems() {
            return Ok(this._dbContext.TaskItems.ToList());
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles ="Manager")]
        public IActionResult GetTaskItem(Guid id) { 
            var taskItem = this._dbContext.TaskItems.FirstOrDefault(x => x.Id == id);
            if (taskItem == null) { return NotFound(); }

            return Ok(taskItem);
        }

        [HttpPost]
        public IActionResult AddTaskItem(AddTaskItemDto addTaskItemDto)
        {
            var taskItems = new TaskItem() { Title = addTaskItemDto.Title , Description = addTaskItemDto.Description, 
             CreatedOnUtc = DateTime.UtcNow, UpdatedOnUtc = DateTime.UtcNow};

            _dbContext.TaskItems.Add(taskItems);
            _dbContext.SaveChanges();

            return Ok(taskItems);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateTaskItem(Guid id,AddTaskItemDto updateTask)
        {
            var item = this._dbContext.TaskItems.FirstOrDefault(x => x.Id == id);
            if(item == null) { return NotFound(); }
            item.Title = updateTask.Title;
            item.Description = updateTask.Description;
            item.CreatedOnUtc = updateTask.CreatedOnUtc;
            item.UpdatedOnUtc = DateTime.UtcNow;
            this._dbContext.SaveChanges();
            return Ok(item);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteTaskItem(Guid id) { 
            var item = _dbContext.TaskItems.FirstOrDefault(x => x.Id == id);
            if(item == null) { return NotFound(); };

            this._dbContext.TaskItems.Remove(item);
            this._dbContext.SaveChanges();
            return Ok();
        }
    }
}
