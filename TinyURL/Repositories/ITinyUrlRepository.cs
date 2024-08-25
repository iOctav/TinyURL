using TinyURL.Models;

namespace TinyURL.Repositories;

public interface ITinyUrlRepository
{
    Task<string> CreateShortUrl(UrlMapping mapping);
    Task<bool> DeleteShortUrl(string shortUrl);
    Task<UrlMapping?> GetUrlMapping(string shortUrl);
    Task<bool> ContainsShortUrl(string shortUrl);
    Task<bool> TryGetUrlMapping(string shortUrl, out UrlMapping? mapping);
    Task<IEnumerable<UrlMapping>> GetAllUrlMappings(int take, int skip);
}