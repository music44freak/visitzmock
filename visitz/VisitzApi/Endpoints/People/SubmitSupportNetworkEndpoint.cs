using System.Net;
using System.Net.Http.Json;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.People;

internal class SubmitSupportNetworkEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    SubmitSupportNetworkJson supportNetwork
) : VisitzBaseEndpoint<bool>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    static readonly string SubmitSupportNetworkPath = "/{0}/{1}/support-network";

    SubmitSupportNetworkJson SupportNetwork => supportNetwork;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(SubmitSupportNetworkPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new()
        {
            Content = JsonContent.Create(SupportNetwork, options: PayloadOptions.MiddlewarePost),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override bool HandleResponse(HttpResponseMessage response, string _)
    {
        return response.StatusCode == HttpStatusCode.OK;
    }
}
