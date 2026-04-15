using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable
internal class GetContactLanguagesService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    IcmContact Contact => (IcmContact)Payload;

    List<ContactLanguageJson> ContactlanguageRecords { get; } = [];

    public static string MakeId(string parentContactId)
    {
        return $"{nameof(GetContactLanguagesService)}|{parentContactId}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.Id),
            ServiceType = typeof(GetContactLanguagesService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, contactlanguages) = await Vpi.GetContactLanguageAsync(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );
        ContactlanguageRecords.AddRange(contactlanguages);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLanguage.SynchronizeAsync(realm, ContactlanguageRecords, Contact.Id)
        );
    }
}
