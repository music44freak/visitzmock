using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class GetContactLegalAuthorityService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    IcmContact Contact => (IcmContact)Payload;

    List<ContactLegalAuthorityJson> ContactLegalAuthorityData { get; } = [];

    public static string MakeId(string parentContactId)
    {
        return $"{nameof(GetContactLegalAuthorityService)}|{parentContactId}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.Id),
            ServiceType = typeof(GetContactLegalAuthorityService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, contactLegalAuthority) = await Vpi.GetContactLegalAuthority(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );

        ContactLegalAuthorityData.AddRange(contactLegalAuthority);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLegalAuthority.SynchronizeAsync(realm, ContactLegalAuthorityData, Contact.Id)
        );
    }
}
