using Realms;
using Visitz.Resources.Localization;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Services.CallDetails;
using Visitz.Services.Messages;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.SafetyAssessments;
using Visitz.Services.Visits;
using VisitzApi;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload;

#nullable enable

/// <summary>
/// Gets all dependent info for a given BusinessObject by concurrently
/// executing individual services. Collects exceptions and rethrows them when
/// finished, so partial success is possible.
/// </summary>
/// <param name="vpi"></param>
/// <param name="prefs"></param>
/// <param name="serviceHandler"></param>
public class GetAllDataForRecordService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiService(vpi, prefs)
{
    ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    IBusinessObject BusinessObject => (IBusinessObject)Payload;

    public static string MakeId(IBusinessObject businessObject)
    {
        return nameof(GetAllDataForRecordService) + "|" + businessObject.Id + "|" + businessObject.EntityType;
    }

    public static StartServiceMessage MakeStartMessage(IBusinessObject businessObject)
    {
        return new()
        {
            ServiceId = MakeId(businessObject),
            ServiceType = typeof(GetAllDataForRecordService),
            Payload = businessObject,
        };
    }

    public override string GetId()
    {
        return MakeId(BusinessObject);
    }

    protected override async Task RunApiServiceAsync()
    {
        List<Exception> exceptions = [];

        await Task.WhenAll(
            GetNotes(exceptions),
            GetVisits(exceptions),
            GetContacts(exceptions),
            GetSupportNetworkItems(exceptions),
            GetAttachments(exceptions),
            GetSafetyAssessments(exceptions),
            GetIncidentConcerns(exceptions),
            GetCallInformation(exceptions),
            GetAdditionalInformation(exceptions)
        );

        //Get Contact related info AFTER fetching all contacts from DBs
        var contacts = BusinessObject.Contacts.Freeze();
        await Task.WhenAll(
            GetContactMedicalBehavioral(contacts, exceptions),
            GetContactlanguages(contacts, exceptions)
        );

        // Get attachment files AFTER other dependent info so we
        // complete text-only downloads sooner
        await GetPartialAttachments(exceptions);

        if (exceptions.Count > 1)
            throw new AggregateException(exceptions);
        else if (exceptions.Count > 0)
            throw exceptions.First();

        BusinessObject.LocalState.LastOpenedBinding = DateTimeOffset.UtcNow;

        ResultCode = Result.Successful;
    }

    static Exception MakeDownloadEx(string kind, Exception ex)
    {
        var msg = string.Format(LocalizedStrings.CaseloadErrorDownload, kind.ToLower());

        return new(msg, ex);
    }

    async Task<Result> GetNotes(List<Exception> exceptions)
    {
        try
        {
            if (BusinessObject.EntityType != EntityType.Memo)
            {
                var startMessage = GetNotesService.MakeStartMessage(
                    BusinessObject.FileNumber,
                    BusinessObject.EntityType
                );
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.Notes, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetVisits(List<Exception> exceptions)
    {
        try
        {
            if (
                BusinessObject.EntityType == EntityType.Case
                && BusinessObject.EntitySubtype == EntitySubtype.ChildServices
            )
            {
                var startMessage = GetVisitsService.MakeStartMessage(BusinessObject.Id);
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.ChildYouthVisits, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetContacts(List<Exception> exceptions)
    {
        try
        {
            var startMessage = GetContactsService.MakeStartMessage(new(BusinessObject));
            return await ServiceHandler.TryRunServiceAsync(startMessage);
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.FamilyMembers, ex));
            return Result.Error;
        }
    }

    async Task<Result> GetSupportNetworkItems(List<Exception> exceptions)
    {
        try
        {
            if (BusinessObject.EntityType != EntityType.Memo)
            {
                var startMessage = GetSupportNetworkService.MakeStartMessage(new(BusinessObject));
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.SupportNetwork, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetAttachments(List<Exception> exceptions)
    {
        try
        {
            var startMessage = GetAttachmentsService.MakeStartMessage(new(BusinessObject));
            return await ServiceHandler.TryRunServiceAsync(startMessage);
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.AttachmentMetadata, ex));
            return Result.Error;
        }
    }

    async Task<Result> GetPartialAttachments(List<Exception> exceptions)
    {
        try
        {
            var startMessage = GetPartialAttachmentsByRangeDownloadService.MakeStartMessage([new(BusinessObject)]);
            return await ServiceHandler.TryRunServiceAsync(startMessage);
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.Attachments, ex));
            return Result.Error;
        }
    }

    async Task<Result> GetSafetyAssessments(List<Exception> exceptions)
    {
        try
        {
            if (BusinessObject.EntityType == EntityType.Incident)
            {
                var startMessage = GetSafetyAssessmentsService.MakeStartMessage(new(BusinessObject));
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.SafetyAssessments, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetIncidentConcerns(List<Exception> exceptions)
    {
        try
        {
            if (BusinessObject.EntityType == EntityType.Incident)
            {
                var startMessage = GetIncidentConcernsService.MakeStartMessage(new(BusinessObject));
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.IncidentConcern, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetCallInformation(List<Exception> exceptions)
    {
        try
        {
            if (BusinessObject.EntityType != EntityType.Case)
            {
                var startMessage = GetCallInformationService.MakeStartMessage(new(BusinessObject));
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.CallInformation, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetAdditionalInformation(List<Exception> exceptions)
    {
        try
        {
            if (
                (BusinessObject.EntityType == EntityType.Incident)
                || (BusinessObject.EntityType == EntityType.Memo)
                || (BusinessObject.EntityType == EntityType.ServiceRequest)
            )
            {
                var startMessage = GetAdditionalInformationService.MakeStartMessage(new(BusinessObject));
                return await ServiceHandler.TryRunServiceAsync(startMessage);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.AdditionalInformation, ex));
            return Result.Error;
        }
        return Result.NoOperation;
    }

    async Task<Result> GetContactMedicalBehavioral(IEnumerable<IcmContact> contacts, List<Exception> exceptions)
    {
        try
        {
            var startMessage = GetContactMedicalBehavioralByRangeService.MakeStartMessage(contacts);
            return await ServiceHandler.TryRunServiceAsync(startMessage);
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.ContactMedicalBehavioral, ex));
            return Result.Error;
        }
    }

    async Task<Result> GetContactlanguages(IEnumerable<IcmContact> contacts, List<Exception> exceptions)
    {
        try
        {
            var startMessage = GetContactLanguagesByRangeService.MakeStartMessage(contacts);
            return await ServiceHandler.TryRunServiceAsync(startMessage);
        }
        catch (Exception ex)
        {
            exceptions.Add(MakeDownloadEx(LocalizedStrings.ContactLanguages, ex));
            return Result.Error;
        }
    }
}
