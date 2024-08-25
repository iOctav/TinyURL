using TinyURL.Host.Models;
using TinyURL.Host.Repositories;

namespace TinyURL.Host.Services;

public class TinyUrlService(ITinyUrlRepository urlRepository, ILogger<TinyUrlService> logger) : ITinyUrlService
{
    private readonly Random _random = new();

    public async Task<string> CreateShortUrl(string longUrl, string? customShortUrl = null)
    {
        logger.LogInformation("Creating short URL for LongUrl: {LongUrl}, CustomShortUrl: {CustomShortUrl}", longUrl, customShortUrl);
        var shortUrl = customShortUrl ?? GenerateShortUrl();
        
        var urlLength = 6;
        while (await urlRepository.TryGetUrlMapping(shortUrl, out var mapping))
        {
            if (customShortUrl != null)
            {
                logger.LogWarning("Custom Short URL already exists: {ShortUrl}", shortUrl);
                if (!mapping?.LongUrl.Equals(longUrl, StringComparison.InvariantCultureIgnoreCase) ?? true) 
                    throw new ArgumentException("Short URL already exists.");
                logger.LogInformation("Custom Short URL already exists for the same Long URL: {ShortUrl}", shortUrl);
                return shortUrl;
            }

            if (urlLength > 10)
            {
                logger.LogError("All short URLs are already used up after trying with length: {Length}", urlLength);
                throw new InvalidOperationException("All Short URLs are already used. Try again later.");
            }
            
            shortUrl = GenerateShortUrl(urlLength++);
            logger.LogInformation("Generated new short URL: {ShortUrl}", shortUrl);
        }

        var urlMapping = new UrlMapping(shortUrl, longUrl);

        await urlRepository.CreateShortUrl(urlMapping);

        return await Task.FromResult(shortUrl);
    }

    public async Task<bool> DeleteShortUrl(string shortUrl)
    {
        logger.LogInformation("Deleting ShortUrl: {ShortUrl}", shortUrl);
        var result = await urlRepository.DeleteShortUrl(shortUrl);

        if (result)
        {
            logger.LogInformation("Successfully deleted ShortUrl: {ShortUrl}", shortUrl);
        }
        else
        {
            logger.LogWarning("Failed to delete ShortUrl: {ShortUrl}. It may not exist.", shortUrl);
        }

        return result;
    }

    public async Task<string?> GetLongUrl(string shortUrl)
    {
        logger.LogInformation("Retrieving LongUrl for ShortUrl: {ShortUrl}", shortUrl);

        var urlMapping = await urlRepository.GetUrlMapping(shortUrl);
        if (urlMapping == null)
        {
            logger.LogWarning("No LongUrl found for ShortUrl: {ShortUrl}", shortUrl);
            return null;
        }

        urlMapping.ClickCount++;
        logger.LogInformation("Incremented click count for ShortUrl: {ShortUrl}, New ClickCount: {ClickCount}", shortUrl, urlMapping.ClickCount);

        return urlMapping.LongUrl;

    }

    public async Task<int> GetUrlStats(string shortUrl)
    {
        logger.LogInformation("Retrieving stats for ShortUrl: {ShortUrl}", shortUrl);

        var urlMapping = await urlRepository.GetUrlMapping(shortUrl);
        var clickCount = urlMapping?.ClickCount ?? 0;

        logger.LogInformation("Retrieved ClickCount: {ClickCount} for ShortUrl: {ShortUrl}", clickCount, shortUrl);

        return clickCount;
    }
    
    public async Task<IEnumerable<URLPair>> GetAllUrls(int take = 10)
    {
        logger.LogInformation("Retrieving all URL mappings.");

        var urlMappings = await urlRepository.GetAllUrlMappings(take);
        var urlPairs = urlMappings.Select(mapping => new URLPair
        {
            ShortUrl = mapping.ShortUrl,
            LongUrl = mapping.LongUrl
        });

        logger.LogInformation("Retrieved {Count} URL mappings.", urlPairs.Count());

        return urlPairs;
    }

    private string GenerateShortUrl(int length = 6)
    {
        logger.LogInformation("Generating short URL with length: {Length}", length);
        
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var shortUrl = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());

        logger.LogInformation("Generated ShortUrl: {ShortUrl}", shortUrl);
        return shortUrl;
    }
}