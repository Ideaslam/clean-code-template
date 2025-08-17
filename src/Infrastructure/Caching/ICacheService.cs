using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

public interface ICacheService
{
	Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
	Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
	Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl, CancellationToken ct = default);
	Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
}