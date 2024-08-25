using Moq;
using TinyURL.Host.Models;
using TinyURL.Host.Repositories;
using TinyURL.Host.Services;
using Microsoft.Extensions.Logging;

namespace TinyURL.UnitTests.Services;

[TestFixture]
public class TinyUrlServiceTests
{
    private Mock<ITinyUrlRepository> _urlRepositoryMock;
    private Mock<ILogger<TinyUrlService>> _loggerMock;
    private TinyUrlService _service;

    [SetUp]
    public void SetUp()
    {
        _urlRepositoryMock = new Mock<ITinyUrlRepository>();
        _loggerMock = new Mock<ILogger<TinyUrlService>>();
        _service = new TinyUrlService(_urlRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task CreateShortUrl_ShouldGenerateNewUrl_WhenCustomShortUrlIsNotProvided()
    {
        const string longUrl = "https://example.com";
        _urlRepositoryMock.Setup(repo => repo.TryGetUrlMapping(It.IsAny<string>(), out It.Ref<UrlMapping>.IsAny))
            .ReturnsAsync(false);

        var result = await _service.CreateShortUrl(longUrl);

        Assert.That(result, Is.Not.Null);
        _urlRepositoryMock.Verify(repo => repo.CreateShortUrl(It.IsAny<UrlMapping>()), Times.Once);
    }

    [Test]
    public void CreateShortUrl_ShouldThrowException_WhenCustomShortUrlExistsWithDifferentLongUrl()
    {
        const string longUrl = "https://example.com";
        const string customShortUrl = "custom";
        var existingMapping = new UrlMapping(customShortUrl, "https://different.com");

        _urlRepositoryMock.Setup(repo => repo.TryGetUrlMapping(customShortUrl, out existingMapping))
            .ReturnsAsync(true);

        var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateShortUrl(longUrl, customShortUrl));
        Assert.That(ex.Message, Is.EqualTo("Short URL already exists."));
    }

    [Test]
    public async Task DeleteShortUrl_ShouldReturnTrue_WhenUrlExists()
    {
        const string shortUrl = "abc123";
        _urlRepositoryMock.Setup(repo => repo.DeleteShortUrl(shortUrl))
            .ReturnsAsync(true);

        var result = await _service.DeleteShortUrl(shortUrl);

        Assert.That(result, Is.True);
        _urlRepositoryMock.Verify(repo => repo.DeleteShortUrl(shortUrl), Times.Once);
    }

    [Test]
    public async Task GetLongUrl_ShouldReturnNull_WhenShortUrlDoesNotExist()
    {
        const string shortUrl = "abc123";
        _urlRepositoryMock.Setup(repo => repo.GetUrlMapping(shortUrl))
            .ReturnsAsync((UrlMapping)null);

        var result = await _service.GetLongUrl(shortUrl);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetUrlStats_ShouldReturnCorrectClickCount()
    {
        const string shortUrl = "abc123";
        var urlMapping = new UrlMapping(shortUrl, "https://example.com") { ClickCount = 5 };

        _urlRepositoryMock.Setup(repo => repo.GetUrlMapping(shortUrl))
            .ReturnsAsync(urlMapping);

        var result = await _service.GetUrlStats(shortUrl);

        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public async Task GetAllUrls_ShouldReturnCorrectNumberOfMappings()
    {
        var urlMappings = new List<UrlMapping>
        {
            new("abc123", "https://example.com"),
            new("xyz789", "https://example2.com")
        };

        _urlRepositoryMock.Setup(repo => repo.GetAllUrlMappings(10))
            .ReturnsAsync(urlMappings);

        var result = await _service.GetAllUrls();

        Assert.That(result.Count(), Is.EqualTo(2));
    }
}