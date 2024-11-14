using System.Net.Http.Json;
using System.Net;
using OneOf;
using OneOf.Types;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace EveStationJanitor.EveApi.Esi;

/// <summary>
/// A <see cref="HttpClient"/> wrapper which handles the nuances of the Eve Online ESI API.
/// </summary>
internal sealed class EveEsiClient
{
    private const int WorkerCount = 8;
    private const int ApiErrorCode = 420;
    private const string ErrorLimitRemainHeader = "X-ESI-Error-Limit-Remain";
    private const string ErrorLimitResetHeader = "X-ESI-Error-Limit-Reset";
    private const string PagesHeader = "X-Pages";

    private readonly HttpClient _client;
    private readonly IEntityTagProvider _entityTagProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public EveEsiClient(
        IHttpClientFactory clientFactory,
        IEntityTagProvider entityTagProvider,
        [FromKeyedServices(JsonSourceGeneratorContext.ServiceKey)] JsonSerializerOptions jsonOptions)
    {
        _client = clientFactory.CreateClient("eve-esi");
        _entityTagProvider = entityTagProvider;
        _jsonOptions = jsonOptions;
    }

    public async Task<GetResult<TResponse>> Get<TResponse>(EveEsiRequest<TResponse> esiRequest) where TResponse : class
    {
        var httpRequestMessage = await esiRequest.CreateHttpRequestMessage(includeEntityTag: false);
        var response = await _client.SendAsync(httpRequestMessage);

        // Handle error limit
        if (IsErrorLimitReached(response)) return new Error<string>("Error limit reached on API.");

        // Handle 304 Not Modified
        if (response.StatusCode == HttpStatusCode.NotModified) return new NotModified();

        // Retry if rate-limited
        if (!response.IsSuccessStatusCode)
        {
            if (await ShouldRetry(response))
            {
                return await Get(esiRequest); // Retry
            }

            return new Error<string>("Error fetching...");
        }

        // Update ETag after successful request
        UpdateEntityTag(response, esiRequest.ETagKey);

        var content = await DeserializeResponseContent<TResponse>(response);
        if (content is null)
        {
            return new Error<string>("Could not deserialize the response.");
        }

        return content;
    }

    public async Task<GetPagedCollectionResult<TResponse>> GetPagedCollection<TResponse>(EveEsiPagedRequest<TResponse> esiRequest)
    {
        var allPages = new List<TResponse>();
        const int currentPage = 1;

        var firstResult = await GetCollectionPage(esiRequest, currentPage, includeEntityTag: true);

        return await firstResult.Match<Task<GetPagedCollectionResult<TResponse>>>(
            async success =>
            {
                allPages.AddRange(success.Results);
                var totalPages = GetTotalPages(success.Response) ?? 0;
                if (totalPages <= 1) return allPages;

                await FetchRemainingPagesAsync(esiRequest, allPages, currentPage + 1, totalPages);

                return allPages;
            },
            async errors => await Task.FromResult(errors),
            async notFound => await Task.FromResult(notFound));
    }

    private async Task FetchRemainingPagesAsync<TResponse>(EveEsiPagedRequest<TResponse> esiRequest, List<TResponse> allPages, int startPage, int totalPages)
    {
        var workers = new List<Task<(List<TResponse> Results, bool HasError)>>();

        var pageChunks = Enumerable.Range(startPage, totalPages - startPage + 1).Chunk(totalPages / WorkerCount);

        foreach (var chunk in pageChunks)
        {
            workers.Add(Task.Run(async () => await FetchPageBatch(esiRequest, chunk)));
        }

        var results = await Task.WhenAll(workers);
        foreach (var (result, hasError) in results)
        {
            if (!hasError) allPages.AddRange(result);
        }
    }

    private async Task<(List<TResponse> Results, bool HasError)> FetchPageBatch<TResponse>(EveEsiPagedRequest<TResponse> baseMessage, IEnumerable<int> pages)
    {
        var results = new List<TResponse>();
        var hasError = false;

        foreach (var page in pages)
        {
            var pageResult = await GetCollectionPage(baseMessage, page, includeEntityTag: false);
            pageResult.Switch(success => results.AddRange(success.Results), _ => hasError = true, _ => { });

            if (hasError) break;
        }

        return (results, hasError);
    }

    private async Task<GetCollectionPageResult<TResponse>> GetCollectionPage<TResponse>(EveEsiPagedRequest<TResponse> request, int page, bool includeEntityTag)
    {
        var esiRequestForPage = request.ForPage(page);
        var httpRequestMessage = await esiRequestForPage.CreateHttpRequestMessage();
        var response = await _client.SendAsync(httpRequestMessage);

        if (IsErrorLimitReached(response)) { return new Error<string>("Error limit reached on API."); }
        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return new NotModified();
        }

        if (!response.IsSuccessStatusCode && await ShouldRetry(response))
        {
            return await GetCollectionPage(request, page, includeEntityTag); // Retry
        }

        var content = await DeserializeResponseContent<List<TResponse>>(response);
        if (content is null)
        {
            return new Error<string>("Could not deserialize response.");
        }

        UpdateEntityTag(response, esiRequestForPage.ETagKey);

        return (content, response);
    }

    private void UpdateEntityTag(HttpResponseMessage response, string? entityTagKey)
    {
        if (entityTagKey is null)
        {
            return;
        }

        var responseEntityTag = response.Headers.ETag?.Tag;
        if (responseEntityTag != null)
        {
            _entityTagProvider.SetEntityTag(entityTagKey, responseEntityTag);
        }
    }

    private static async Task<bool> ShouldRetry(HttpResponseMessage response)
    {
        var (errorsRemaining, secondsUntilReset) = GetErrorLimitInfo(response);

        if (errorsRemaining.HasValue && secondsUntilReset.HasValue)
        {
            var waitTime = CalculateRetryWait(errorsRemaining.Value, secondsUntilReset.Value);
            await Task.Delay(waitTime);
            return true;
        }
        return false;
    }

    private async Task<T?> DeserializeResponseContent<T>(HttpResponseMessage response) where T : class
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsErrorLimitReached(HttpResponseMessage response)
    {
        return (int)response.StatusCode == ApiErrorCode;
    }

    private static TimeSpan CalculateRetryWait(int errorsRemaining, int secondsUntilReset)
    {
        return TimeSpan.FromSeconds((secondsUntilReset / (double)errorsRemaining) + 1);
    }

    private static int? GetTotalPages(HttpResponseMessage response)
    {
        var pagesHeader = response.Headers.GetValues(PagesHeader).FirstOrDefault();
        return int.TryParse(pagesHeader, out var pages) ? pages : null;
    }

    private static ErrorLimitInfo GetErrorLimitInfo(HttpResponseMessage response)
    {
        return new ErrorLimitInfo(
            GetHeaderIntValue(response, ErrorLimitRemainHeader),
            GetHeaderIntValue(response, ErrorLimitResetHeader));
    }

    private static int? GetHeaderIntValue(HttpResponseMessage response, string headerName)
    {
        return response.Headers.TryGetValues(headerName, out var values) && int.TryParse(values.FirstOrDefault(), out var result) ? result : null;
    }

    private readonly record struct ErrorLimitInfo(int? ErrorsRemaining, int? SecondsUntilLimitReset);
}

[GenerateOneOf]
public partial class GetPagedCollectionResult<T> : OneOfBase<List<T>, Error<string>, NotModified>;

[GenerateOneOf]
public partial class GetResult<T> : OneOfBase<T, Error<string>, NotModified>;

[GenerateOneOf]
public partial class GetCollectionPageResult<T> : OneOfBase<(List<T> Results, HttpResponseMessage Response), Error<string>, NotModified>;
