using DotNetAzure_API.Models.Entities;

namespace DotNetAzure_API.Services
{
    public interface ITaskService
    {
        Task<TaskItem?> GetTaskAsync(Guid taskId); 
    }
}
