using Grpc.Core;

namespace TinyURL.Host.Services;

public class TinyUrlGrpcService(ITinyUrlService tinyUrlService, ILogger<TinyUrlGrpcService> logger)
    : TinyURLService.TinyURLServiceBase
{
    public override async Task<CreateShortURLResponse> CreateShortURL(CreateShortURLRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received CreateShortURL request for LongUrl: {LongUrl}", request.LongUrl);

        try
        {
            var shortUrl = await tinyUrlService.CreateShortUrl(request.LongUrl, 
                request.HasCustomShortUrl ? request.CustomShortUrl : null);
            logger.LogInformation("Created ShortUrl: {ShortUrl} for LongUrl: {LongUrl}", shortUrl, request.LongUrl);
            return new CreateShortURLResponse { ShortUrl = shortUrl };
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Error occurred while creating ShortUrl for LongUrl: {LongUrl}", request.LongUrl);
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating ShortUrl for LongUrl: {LongUrl}", request.LongUrl);
            throw new RpcException(new Status(StatusCode.Internal, "Error creating short URL"));
        }
    }

    public override async Task<DeleteShortURLResponse> DeleteShortURL(DeleteShortURLRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received DeleteShortURL request for ShortUrl: {ShortUrl}", request.ShortUrl);

        try
        {
            var success = await tinyUrlService.DeleteShortUrl(request.ShortUrl);
            if (success)
            {
                logger.LogInformation("Successfully deleted ShortUrl: {ShortUrl}", request.ShortUrl);
            }
            else
            {
                logger.LogWarning("Failed to delete ShortUrl: {ShortUrl}. URL may not exist.", request.ShortUrl);
            }

            return new DeleteShortURLResponse { Success = success };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting ShortUrl: {ShortUrl}", request.ShortUrl);
            throw new RpcException(new Status(StatusCode.Internal, "Error deleting short URL"));
        }
    }

    public override async Task<GetLongURLResponse> GetLongURL(GetLongURLRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received GetLongURL request for ShortUrl: {ShortUrl}", request.ShortUrl);

        try
        {
            var longUrl = await tinyUrlService.GetLongUrl(request.ShortUrl);
            if (string.IsNullOrEmpty(longUrl))
            {
                logger.LogWarning("No LongUrl found for ShortUrl: {ShortUrl}", request.ShortUrl);
                throw new RpcException(new Status(StatusCode.NotFound, "Short URL not found"));
            }

            logger.LogInformation("Retrieved LongUrl: {LongUrl} for ShortUrl: {ShortUrl}", longUrl, request.ShortUrl);
            return new GetLongURLResponse { LongUrl = longUrl };
        }
        catch (RpcException)
        {
            throw; // Let the RpcException bubble up as it has the correct status code
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving LongUrl for ShortUrl: {ShortUrl}", request.ShortUrl);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving long URL"));
        }
    }

    public override async Task<GetURLStatsResponse> GetURLStats(GetURLStatsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received GetURLStats request for ShortUrl: {ShortUrl}", request.ShortUrl);

        try
        {
            var clickCount = await tinyUrlService.GetUrlStats(request.ShortUrl);
            logger.LogInformation("Retrieved ClickCount: {ClickCount} for ShortUrl: {ShortUrl}", clickCount, request.ShortUrl);
            return new GetURLStatsResponse { ClickCount = clickCount };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving stats for ShortUrl: {ShortUrl}", request.ShortUrl);
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving URL stats"));
        }
    }
    
    public override async Task<GetAllURLsResponse> GetAllURLs(GetAllURLsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received GetAllURLs request");
        try
        {
            var urlPairs = request.Filter == null 
                ? await tinyUrlService.GetAllUrls()
                : await tinyUrlService.GetAllUrls(request.Filter.Take);
            var response = new GetAllURLsResponse();
            response.Urls.AddRange(urlPairs.Select(url => new URLPair
            {
                ShortUrl = url.ShortUrl,
                LongUrl = url.LongUrl
            }));

            logger.LogInformation("Retrieved {Count} URLs", urlPairs.Count());
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all URLs");
            throw new RpcException(new Status(StatusCode.Internal, "Error retrieving all URLs"));
        }
    }
}
