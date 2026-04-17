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

internal class GetContactEducationService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    IcmContact Contact => (IcmContact)Payload;

    List<ContactEducationJson> ContactEducationData { get; } = [];

    public static string MakeId(string parentContactId)
    {
        return $"{nameof(GetContactEducationService)}|{parentContactId}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.Id),
            ServiceType = typeof(GetContactEducationService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (totalCount, contactEducation) = await Vpi.GetContactEducation(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );

        ContactEducationData.AddRange(contactEducation);

        return totalCount;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactEducation.SynchronizeAsync(realm, ContactEducationData, Contact.Id)
        );
    }
}
