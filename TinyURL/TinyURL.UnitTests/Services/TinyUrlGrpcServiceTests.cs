using Grpc.Core;
using Moq;
using TinyURL.Host.Services;
using Microsoft.Extensions.Logging;

namespace TinyURL.UnitTests.Services;

[TestFixture]
public class TinyUrlGrpcServiceTests
{
    private Mock<ITinyUrlService> _tinyUrlServiceMock;
    private Mock<ILogger<TinyUrlGrpcService>> _loggerMock;
    private TinyUrlGrpcService _grpcService;

    [SetUp]
    public void SetUp()
    {
        _tinyUrlServiceMock = new Mock<ITinyUrlService>();
        _loggerMock = new Mock<ILogger<TinyUrlGrpcService>>();
        _grpcService = new TinyUrlGrpcService(_tinyUrlServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task CreateShortURL_ShouldReturnShortUrl_WhenRequestIsValid()
    {
        var request = new CreateShortURLRequest { LongUrl = "https://example.com" };
        _tinyUrlServiceMock.Setup(service => service.CreateShortUrl(request.LongUrl, null))
            .ReturnsAsync("short123");

        var response = await _grpcService.CreateShortURL(request, null);

        Assert.That(response.ShortUrl, Is.EqualTo("short123"));
    }

    [Test]
    public void CreateShortURL_ShouldThrowRpcException_WhenArgumentExceptionIsThrown()
    {
        var request = new CreateShortURLRequest { LongUrl = "https://example.com", CustomShortUrl = "custom" };
        _tinyUrlServiceMock.Setup(service => service.CreateShortUrl(request.LongUrl, "custom"))
            .ThrowsAsync(new ArgumentException("Short URL already exists."));

        var ex = Assert.ThrowsAsync<RpcException>(() => _grpcService.CreateShortURL(request, null));
        Assert.Multiple(() =>
        {
            Assert.That(ex.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
            Assert.That(ex.Status.Detail, Is.EqualTo("Short URL already exists."));
        });
    }

    [Test]
    public async Task DeleteShortURL_ShouldReturnSuccess_WhenUrlIsDeleted()
    {
        var request = new DeleteShortURLRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.DeleteShortUrl(request.ShortUrl))
            .ReturnsAsync(true);

        var response = await _grpcService.DeleteShortURL(request, null);

        Assert.That(response.Success, Is.True);
    }

    [Test]
    public void DeleteShortURL_ShouldThrowRpcException_WhenExceptionIsThrown()
    {
        var request = new DeleteShortURLRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.DeleteShortUrl(request.ShortUrl))
            .ThrowsAsync(new Exception("Error deleting URL."));

        var ex = Assert.ThrowsAsync<RpcException>(() => _grpcService.DeleteShortURL(request, null));
        Assert.Multiple(() =>
        {
            Assert.That(ex.StatusCode, Is.EqualTo(StatusCode.Internal));
            Assert.That(ex.Status.Detail, Is.EqualTo("Error deleting short URL"));
        });
    }

    [Test]
    public async Task GetLongURL_ShouldReturnLongUrl_WhenShortUrlExists()
    {
        var request = new GetLongURLRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.GetLongUrl(request.ShortUrl))
            .ReturnsAsync("https://example.com");

        var response = await _grpcService.GetLongURL(request, null);

        Assert.That(response.LongUrl, Is.EqualTo("https://example.com"));
    }

    [Test]
    public void GetLongURL_ShouldThrowRpcException_WhenShortUrlNotFound()
    {
        var request = new GetLongURLRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.GetLongUrl(request.ShortUrl))
            .ReturnsAsync((string)null);

        var ex = Assert.ThrowsAsync<RpcException>(() => _grpcService.GetLongURL(request, null));
        Assert.That(ex.StatusCode, Is.EqualTo(StatusCode.NotFound));
        Assert.That(ex.Status.Detail, Is.EqualTo("Short URL not found"));
    }

    [Test]
    public async Task GetURLStats_ShouldReturnClickCount_WhenShortUrlExists()
    {
        var request = new GetURLStatsRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.GetUrlStats(request.ShortUrl))
            .ReturnsAsync(10);

        var response = await _grpcService.GetURLStats(request, null);

        Assert.That(response.ClickCount, Is.EqualTo(10));
    }

    [Test]
    public void GetURLStats_ShouldThrowRpcException_WhenExceptionIsThrown()
    {
        var request = new GetURLStatsRequest { ShortUrl = "short123" };
        _tinyUrlServiceMock.Setup(service => service.GetUrlStats(request.ShortUrl))
            .ThrowsAsync(new Exception("Error retrieving stats."));

        var ex = Assert.ThrowsAsync<RpcException>(() => _grpcService.GetURLStats(request, null));
        Assert.Multiple(() =>
        {
            Assert.That(ex.StatusCode, Is.EqualTo(StatusCode.Internal));
            Assert.That(ex.Status.Detail, Is.EqualTo("Error retrieving URL stats"));
        });
    }

    [Test]
    public async Task GetAllURLs_ShouldReturnAllUrls_WhenRequestIsValid()
    {
        var request = new GetAllURLsRequest();
        var urlPairs = new List<URLPair>
        {
            new() { ShortUrl = "short1", LongUrl = "https://example1.com" },
            new() { ShortUrl = "short2", LongUrl = "https://example2.com" }
        };

        _tinyUrlServiceMock.Setup(service => service.GetAllUrls(10))
            .ReturnsAsync(urlPairs);

        var response = await _grpcService.GetAllURLs(request, null);

        Assert.Multiple(() =>
        {
            Assert.That(response.Urls, Has.Count.EqualTo(2));
            Assert.That(response.Urls[0].ShortUrl, Is.EqualTo("short1"));
            Assert.That(response.Urls[0].LongUrl, Is.EqualTo("https://example1.com"));
        });
    }

    [Test]
    public void GetAllURLs_ShouldThrowRpcException_WhenExceptionIsThrown()
    {
        var request = new GetAllURLsRequest();
        _tinyUrlServiceMock.Setup(service => service.GetAllUrls(10))
            .ThrowsAsync(new Exception("Error retrieving URLs."));

        var ex = Assert.ThrowsAsync<RpcException>(() => _grpcService.GetAllURLs(request, null));
        Assert.Multiple(() =>
        {
            Assert.That(ex.StatusCode, Is.EqualTo(StatusCode.Internal));
            Assert.That(ex.Status.Detail, Is.EqualTo("Error retrieving all URLs"));
        });
    }
}