namespace TinyURL.Host.Services;

public interface ITinyUrlService
{
    Task<string> CreateShortUrl(string longUrl, string? customShortUrl = null);
    Task<bool> DeleteShortUrl(string shortUrl);
    Task<string?> GetLongUrl(string shortUrl);
    Task<int> GetUrlStats(string shortUrl);
    Task<IEnumerable<URLPair>> GetAllUrls(int take = 10);
}
