﻿using EveStationJanitor.Authentication;
using Flurl;
using System.Net.Http.Headers;

namespace EveStationJanitor.EveApi.Esi;

/// <summary>
/// Handles the creation of <see cref="HttpRequestMessage"/>for the EVE Online API. This class can authenticate requests and apply ETag headers.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
internal sealed class EveEsiRequest<TResponse>(IEveEsiEndpointSpec specification, IEntityTagProvider entityTagProvider, IBearerTokenProvider? tokenProvider = null)
{
    private readonly Dictionary<string, object?> _queryParameters = [];

    public string ETagKey => BuildUri().ToString();

    public async Task<HttpRequestMessage> CreateHttpRequestMessage(bool includeEntityTag = true)
    {
        var uri = BuildUri();
        var message = new HttpRequestMessage(specification.HttpMethod, uri);

        if (tokenProvider != null)
        {
            var token = await tokenProvider.GetToken();
            if (token is not null)
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        if (includeEntityTag)
        {
            var entityTag = entityTagProvider.GetEntityTag(ETagKey);
            if (entityTag is not null)
            {
                message.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(entityTag));
            }
        }

        return message;
    }

    private Uri BuildUri()
    {
        var path = specification.RelativeUrlPath;
        var uriBuilder = EveEsiRequest.UriBase.AppendPathSegment(path)
            .SetQueryParams(specification.QueryKeyValues)
            .AppendQueryParam("datasource", "tranquility");

        foreach (var (key, value) in _queryParameters)
        {
            uriBuilder.AppendQueryParam(key, value);
        }

        return uriBuilder.ToUri();
    }

    public void AddQueryParameter(string key, object? value)
    {
        _queryParameters.Add(key, value);
    }
}

internal static class EveEsiRequest
{
    internal static readonly Uri UriBase = new("https://esi.evetech.net/latest/", UriKind.Absolute);
}
