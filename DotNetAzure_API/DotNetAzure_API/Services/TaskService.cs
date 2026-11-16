using DotNetAzure_API.Data;
using DotNetAzure_API.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DotNetAzure_API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TaskService> _logger;
        private readonly TaskRedisCache _redisCache;
        public TaskService(ApplicationDbContext db,
                            ILogger<TaskService> logger,
                            TaskRedisCache cache)
        {
            _db = db;
            _logger = logger;
            _redisCache = cache;
        }

        public async Task<TaskItem?> GetTaskAsync(Guid taskId)
        {
            var key = $"product:{taskId}";

            // 1. Try Redis first.try     {
            try
            {
                var json = await _redisCache.GetStringAsync(key);

                if (json is not null)
                {
                    _logger.LogInformation(
                        "Product {ProductId} retrieved from Redis", taskId);

                    return JsonSerializer.Deserialize<TaskItem>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Redis read failed for product {ProductId}", taskId);
            }

            // 2. Redis miss or failure: query SQL.
            var product = await _db.TaskItems
                         .FirstOrDefaultAsync(p => p.Id == taskId);
            _logger.LogInformation(
                         "Product {ProductId} retrieved from DB", taskId);
            if (product is null)
                return null;

            // 3. Cache the SQL result. Cache failure should not fail the request.
            try
            {
                await _redisCache.SetStringAsync(
                    key,
                    JsonSerializer.Serialize(product),
                    TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex, "Redis write failed for product {ProductId}", taskId);
            }

            return product;
        }
    }

}
