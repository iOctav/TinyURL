using System.Collections.Concurrent;
using TinyURL.Host.Models;

namespace TinyURL.Host.Repositories;

public class TinyUrlRepository : ITinyUrlRepository
{
    private readonly ConcurrentDictionary<string, UrlMapping> _urlStore = new();

    public Task<bool> ContainsShortUrl(string shortUrl)
    {
        var contains = _urlStore.ContainsKey(shortUrl);
        return Task.FromResult(contains);
    }

    public Task<bool> TryGetUrlMapping(string shortUrl, out UrlMapping? mapping)
    {
        var success = _urlStore.TryGetValue(shortUrl, out mapping);
        return Task.FromResult(success);
    }

    public Task<string> CreateShortUrl(UrlMapping mapping)
    {
        if (_urlStore.TryAdd(mapping.ShortUrl, mapping))
        {
            return Task.FromResult(mapping.ShortUrl);
        }

        throw new InvalidOperationException("Short URL already exists or could not be created.");
    }

    public Task<bool> DeleteShortUrl(string shortUrl)
    {
        var removed = _urlStore.TryRemove(shortUrl, out _);
        return Task.FromResult(removed);
    }

    public Task<UrlMapping?> GetUrlMapping(string shortUrl)
    {
        _urlStore.TryGetValue(shortUrl, out var mapping);
        return Task.FromResult(mapping);
    }

    public Task<IEnumerable<UrlMapping>> GetAllUrlMappings(int take)
    {
        var allMappings = _urlStore.Values.TakeLast(take).AsEnumerable();
        return Task.FromResult(allMappings);
    }
}