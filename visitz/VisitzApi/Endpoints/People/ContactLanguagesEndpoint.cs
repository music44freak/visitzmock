using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;
#nullable enable
namespace VisitzApi.Endpoints.People;

internal class ContactLanguagesEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    string contactId,
    Pagination? pagination = null
)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ContactLanguageJson>)>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId, contactId)
    )
{
    static readonly string ContactsPath = "/{0}/{1}/contacts/{2}/languages";
    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId, string contactId)
    {
        return string.Format(ContactsPath, recordType.ToString().ToLowerInvariant(), rowId, contactId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<ContactLanguageJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ContactLanguageJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
