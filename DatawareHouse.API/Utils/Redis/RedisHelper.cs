using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace DataWarehouse.Utils.Redis;

public class RedisHelper
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisHelper(IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnection");
        if (string.IsNullOrEmpty(redisConnectionString))
            throw new ArgumentException("Redis connection string is missing in the configuration.");

        var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
        configurationOptions.AbortOnConnectFail = false;
        _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);

        _serializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };
    }

    public IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    public async Task<T?> GetCacheAsync<T>(string key) where T : class
    {
        var cachedData = await GetDatabase().StringGetAsync(key);
        if (!cachedData.IsNullOrEmpty)
            return JsonSerializer.Deserialize<T>(cachedData!, _serializerOptions);

        return null;
    }

    public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedData = JsonSerializer.Serialize(value, _serializerOptions);
        await GetDatabase().StringSetAsync(key, serializedData, expiry);
    }

    public async Task DeleteCacheAsync(string key) => await GetDatabase().KeyDeleteAsync(key);

    public async Task DeleteKeysByPatternAsync(string pattern)
    {
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
        foreach (var key in server.Keys(pattern: pattern))
        {
            await DeleteCacheAsync(key);
        }
    }

    public async Task UpdateListCacheAsync<T>(string key, Func<List<T>, List<T>> updateFunc, TimeSpan? expiry = null) where T : class
    {
        var list = (await GetCacheAsync<List<T>>(key)) ?? new List<T>();
        var updatedList = updateFunc(list);
        await SetCacheAsync(key, updatedList, expiry);
    }

    public async Task RemoveFromListCacheAsync<T>(string key, Func<List<T>, List<T>> removeFunc, TimeSpan? expiry = null) where T : class
    {
        var list = await GetCacheAsync<List<T>>(key);
        if (list == null) return;

        var updatedList = removeFunc(list);
        await SetCacheAsync(key, updatedList, expiry);
    }

    public async Task<decimal?> GetDecimalCacheAsync(string key)
    {
        var cachedData = await GetDatabase().StringGetAsync(key);
        return decimal.TryParse(cachedData, out var result) ? result : null;
    }

    public async Task SetDecimalCacheAsync(string key, decimal value, TimeSpan? expiry = null)
    {
        await GetDatabase().StringSetAsync(key, value.ToString(CultureInfo.InvariantCulture), expiry);
    }
}
