using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.People;

internal class ContactEducationEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    string contactId,
    Pagination? pagination = null
)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ContactEducationJson>)>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId, contactId)
    )
{
    static readonly string ContactEducationPath = "/{0}/{1}/contact/{2}/education";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId, string contactId)
    {
        return string.Format(ContactEducationPath, recordType.ToString().ToLowerInvariant(), rowId, contactId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<ContactEducationJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ContactEducationJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
