using Azure.Identity;
using StackExchange.Redis;
using Microsoft.Azure.StackExchangeRedis;

namespace DotNetAzure_API.Services
{
    public sealed class TaskRedisCache : IAsyncDisposable
    {
        private readonly Lazy<Task<ConnectionMultiplexer>> _connection;

        public TaskRedisCache(IConfiguration configuration)
        {
            var endpoint = configuration["Redis:Endpoint"]
                ?? throw new InvalidOperationException(
                    "Redis:Endpoint is missing from configuration.");

            _connection = new Lazy<Task<ConnectionMultiplexer>>(
                () => ConnectAsync(endpoint));
        }

        private static async Task<ConnectionMultiplexer> ConnectAsync(
            string endpoint)
        {
            var options = new ConfigurationOptions
            {
                Protocol = RedisProtocol.Resp3,
                AbortOnConnectFail = false,
                BacklogPolicy = BacklogPolicy.Default
            };

            options.EndPoints.Add(endpoint);

            await options.ConfigureForAzureWithTokenCredentialAsync(
                new AzureCliCredential());
            var connection = ConnectionMultiplexer.Connect(options);
            Console.WriteLine(connection.IsConnected);
            return await ConnectionMultiplexer.ConnectAsync(options);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var db = (await _connection.Value).GetDatabase();
            var value = await db.StringGetAsync(key);

            return value.IsNull ? null : value.ToString();
        }

        public async Task SetStringAsync(
            string key,
            string value,
            TimeSpan expiry)
        {
            var db = (await _connection.Value).GetDatabase();
            await db.StringSetAsync(key, value, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            var db = (await _connection.Value).GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_connection.IsValueCreated)
                return;

            try
            {
                var connection = await _connection.Value;
                await connection.CloseAsync();
                connection.Dispose();
            }
            catch
            {
                // Do not mask application shutdown with a cache cleanup error.            }
            }
        }

    }
}
