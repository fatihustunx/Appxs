using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System;

namespace App.Appxs.Apps.Caching;

public interface ICachingRequest
{
    string CacheKey { get; }
    bool BypassCache { get; }
    string? CacheGroupKey { get; }
    //TimeSpan? SlidingExpiration { get; }
}

public interface ICacheRemoverRequest
{
    //string? CacheKey { get; }
    string CacheGroupKey { get; }
    bool BypassCache { get; }
}

public class CacheSettings
{
    public int SlidingExpiration { get; set; }
}

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICachingRequest
{
    private readonly IDistributedCache _cache;
    private readonly CacheSettings cacheSettings;

    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, IConfiguration configuration,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        this._cache = cache;

        this.cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>()
            ?? throw new InvalidOperationException();

        this._logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            return await next();
        }

        TResponse res;

        byte[]? cachedRes = await _cache.GetAsync(request.CacheKey, cancellationToken);

        if (cachedRes != null)
        {
            res = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedRes));

            _logger.LogInformation($"Fetched from cache -> {request.CacheKey}");
        }
        else
        {
            res = await getResAndAddToCache(request, next, cancellationToken);
        }

        return res;
    }

    private async Task<TResponse> getResAndAddToCache(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse res = await next();

        TimeSpan slidingExpiration = TimeSpan.FromDays(cacheSettings.SlidingExpiration);
        //TimeSpan slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromDays(cacheSettings.SlidingExpiration);

        DistributedCacheEntryOptions cacheOptions = new() { SlidingExpiration = slidingExpiration };

        byte[] serializedData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(res));

        await _cache.SetAsync(request.CacheKey, serializedData, cacheOptions, cancellationToken);

        _logger.LogInformation($"Added to cache -> {request.CacheKey}");

        if (request.CacheGroupKey != null)
        {
            await addCacheKeyToGroup(request, slidingExpiration, cancellationToken);
        }

        return res;
    }

    private async Task addCacheKeyToGroup(TRequest request, TimeSpan slidingExpiration, CancellationToken cancellationToken)
    {
        byte[]? cacheGroupCache = await _cache.GetAsync(key: request.CacheGroupKey!, cancellationToken);
        HashSet<string> cacheKeysInGroup;
        if (cacheGroupCache != null)
        {
            cacheKeysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cacheGroupCache))!;
            if (!cacheKeysInGroup.Contains(request.CacheKey))
                cacheKeysInGroup.Add(request.CacheKey);
        }
        else
            cacheKeysInGroup = new HashSet<string>(new[] { request.CacheKey });
        byte[] newCacheGroupCache = JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup);

        byte[]? cacheGroupCacheSlidingExpirationCache = await _cache.GetAsync(
            key: $"{request.CacheGroupKey}SlidingExpiration",
            cancellationToken
        );
        int? cacheGroupCacheSlidingExpirationValue = null;
        if (cacheGroupCacheSlidingExpirationCache != null)
            cacheGroupCacheSlidingExpirationValue = Convert.ToInt32(Encoding.Default.GetString(cacheGroupCacheSlidingExpirationCache));
        if (cacheGroupCacheSlidingExpirationValue == null || slidingExpiration.TotalSeconds > cacheGroupCacheSlidingExpirationValue)
            cacheGroupCacheSlidingExpirationValue = Convert.ToInt32(slidingExpiration.TotalSeconds);
        byte[] serializeCachedGroupSlidingExpirationData = JsonSerializer.SerializeToUtf8Bytes(cacheGroupCacheSlidingExpirationValue);

        DistributedCacheEntryOptions cacheOptions =
            new() { SlidingExpiration = TimeSpan.FromSeconds(Convert.ToDouble(cacheGroupCacheSlidingExpirationValue)) };

        await _cache.SetAsync(key: request.CacheGroupKey!, newCacheGroupCache, cacheOptions, cancellationToken);
        _logger.LogInformation($"Added to Cache -> {request.CacheGroupKey}");

        await _cache.SetAsync(
            key: $"{request.CacheGroupKey}SlidingExpiration",
            serializeCachedGroupSlidingExpirationData,
            cacheOptions,
            cancellationToken
        );
        _logger.LogInformation($"Added to Cache -> {request.CacheGroupKey}SlidingExpiration");
    }
}

public class CacheRemovingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheRemoverRequest
{
    private readonly IDistributedCache _cache;

    private readonly ILogger<CacheRemovingBehavior<TRequest, TResponse>> _logger;

    public CacheRemovingBehavior(IDistributedCache cache, ILogger<CacheRemovingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            return await next();
        }

        TResponse res = await next();


        if (request.CacheGroupKey != null)
        {
            byte[]? cachedGroup = await _cache.GetAsync(request.CacheGroupKey, cancellationToken);
            if (cachedGroup != null)
            {
                HashSet<string> keysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cachedGroup))!;
                foreach (string key in keysInGroup)
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                }

                await _cache.RemoveAsync(request.CacheGroupKey, cancellationToken);
                await _cache.RemoveAsync(key: $"{request.CacheGroupKey}SlidingExpiration", cancellationToken);
            }
        }

        //if (request.CacheKey != null)
        //{
        //    await _cache.RemoveAsync(request.CacheKey, cancellationToken);

        //    _logger.LogInformation($"Removed from cache -> {request.CacheKey}");
        //}

        return res;
    }
}