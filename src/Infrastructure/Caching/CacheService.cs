using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Caching;

public class CacheService : ICacheService
{
	private readonly IDistributedCache _distributedCache;
	private readonly IMemoryCache _memoryCache;
	private readonly ILogger<CacheService> _logger;
	private readonly bool _redisEnabled;

	public CacheService(IDistributedCache distributedCache, IMemoryCache memoryCache, ILogger<CacheService> logger)
	{
		_distributedCache = distributedCache;
		_memoryCache = memoryCache;
		_logger = logger;
		_redisEnabled = distributedCache.GetType().FullName?.Contains("Redis", StringComparison.OrdinalIgnoreCase) == true;
	}

	public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
	{
		try
		{
			if (_redisEnabled)
			{
				var bytes = await _distributedCache.GetAsync(key, ct);
				if (bytes is null)
					return default;
				return JsonSerializer.Deserialize<T>(bytes);
			}
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis unavailable, using memory cache for {Key}", key);
		}
		return _memoryCache.TryGetValue(key, out T? value) ? value : default;
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
	{
		try
		{
			if (_redisEnabled)
			{
				var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
				await _distributedCache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
				return;
			}
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Redis unavailable, using memory cache for {Key}", key);
		}
		_memoryCache.Set(key, value, ttl);
	}

	public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl, CancellationToken ct = default)
	{
		var cached = await GetAsync<T>(key, ct);
		if (cached is not null)
			return cached;
		var value = await factory();
		await SetAsync(key, value!, ttl, ct);
		return value;
	}

	public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
	{
		// Simple in-memory eviction by pattern. For Redis, recommend server-assisted scanning in production.
		if (!_redisEnabled)
		{
			if (_memoryCache is MemoryCache mem)
			{
				foreach (var entry in mem as IEnumerable<KeyValuePair<object, object?>> ?? Array.Empty<KeyValuePair<object, object?>>())
				{
					if (entry.Key is string s && s.Contains(pattern, StringComparison.OrdinalIgnoreCase))
						mem.Remove(s);
				}
			}
			return;
		}
		_logger.LogInformation("Requested Redis pattern eviction for {Pattern}. Consider implementing Redis key scan.", pattern);
		await Task.CompletedTask;
	}
}