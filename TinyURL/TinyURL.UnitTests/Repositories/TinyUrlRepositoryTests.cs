using TinyURL.Host.Models;
using TinyURL.Host.Repositories;

namespace TinyURL.UnitTests.Repositories;

[TestFixture]
public class TinyUrlRepositoryTests
{
    private TinyUrlRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new TinyUrlRepository();
    }

    [Test]
    public async Task ContainsShortUrl_ShouldReturnFalse_WhenShortUrlDoesNotExist()
    {
        var result = await _repository.ContainsShortUrl("nonexistent");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ContainsShortUrl_ShouldReturnTrue_WhenShortUrlExists()
    {
        var mapping = new UrlMapping("short123", "https://example.com");
        await _repository.CreateShortUrl(mapping);

        var result = await _repository.ContainsShortUrl("short123");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task TryGetUrlMapping_ShouldReturnFalse_WhenShortUrlDoesNotExist()
    {
        var success = await _repository.TryGetUrlMapping("nonexistent", out var mapping);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(mapping, Is.Null);
        });
    }

    [Test]
    public async Task TryGetUrlMapping_ShouldReturnTrue_WhenShortUrlExists()
    {
        var mapping = new UrlMapping("short123", "https://example.com");
        await _repository.CreateShortUrl(mapping);

        var success = await _repository.TryGetUrlMapping("short123", out var retrievedMapping);
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(retrievedMapping, Is.Not.Null);
            Assert.That(retrievedMapping?.ShortUrl, Is.EqualTo("short123"));
            Assert.That(retrievedMapping?.LongUrl, Is.EqualTo("https://example.com"));
        });
    }

    [Test]
    public async Task CreateShortUrl_ShouldThrowInvalidOperationException_WhenShortUrlAlreadyExists()
    {
        var mapping = new UrlMapping("short123", "https://example.com");
        await _repository.CreateShortUrl(mapping);

        Assert.ThrowsAsync<InvalidOperationException>(() => _repository.CreateShortUrl(mapping));
    }

    [Test]
    public async Task CreateShortUrl_ShouldAddMapping_WhenShortUrlIsNew()
    {
        var mapping = new UrlMapping("short123", "https://example.com");

        var result = await _repository.CreateShortUrl(mapping);

        Assert.That(result, Is.EqualTo("short123"));

        var contains = await _repository.ContainsShortUrl("short123");
        Assert.That(contains, Is.True);
    }

    [Test]
    public async Task DeleteShortUrl_ShouldReturnFalse_WhenShortUrlDoesNotExist()
    {
        var result = await _repository.DeleteShortUrl("nonexistent");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteShortUrl_ShouldReturnTrue_WhenShortUrlExists()
    {
        var mapping = new UrlMapping("short123", "https://example.com");
        await _repository.CreateShortUrl(mapping);

        var result = await _repository.DeleteShortUrl("short123");

        Assert.That(result, Is.True);

        var contains = await _repository.ContainsShortUrl("short123");
        Assert.That(contains, Is.False);
    }

    [Test]
    public async Task GetUrlMapping_ShouldReturnNull_WhenShortUrlDoesNotExist()
    {
        var result = await _repository.GetUrlMapping("nonexistent");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetUrlMapping_ShouldReturnMapping_WhenShortUrlExists()
    {
        var mapping = new UrlMapping("short123", "https://example.com");
        await _repository.CreateShortUrl(mapping);

        var result = await _repository.GetUrlMapping("short123");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ShortUrl, Is.EqualTo("short123"));
            Assert.That(result.LongUrl, Is.EqualTo("https://example.com"));
        });
    }

    [Test]
    public async Task GetAllUrlMappings_ShouldReturnCorrectMappings_WhenThereAreMappings()
    {
        var mapping1 = new UrlMapping("short123", "https://example.com");
        var mapping2 = new UrlMapping("short456", "https://example2.com");
        await _repository.CreateShortUrl(mapping1);
        await _repository.CreateShortUrl(mapping2);

        var result = await _repository.GetAllUrlMappings(10);
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.ToList(), Does.Contain(mapping1));
            Assert.That(result.ToList(), Does.Contain(mapping2));
        });
    }
}