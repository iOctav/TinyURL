namespace TinyURL.Models;

public class UrlMapping(string shortUrl, string longUrl)
{
    public string ShortUrl { get; set; } = shortUrl;
    public string LongUrl { get; set; } = longUrl;
    public int ClickCount { get; set; } = 0;
}
