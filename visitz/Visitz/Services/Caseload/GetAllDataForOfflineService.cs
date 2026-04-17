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
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.Caseload
{
    public class GetAllDataForOfflineService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
        : VisitzApiService(vpi, prefs)
    {
        public static string MakeId()
        {
            return nameof(GetAllDataForOfflineService);
        }

        public static StartServiceMessage MakeStartMessage(bool forceDownload = false)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetAllDataForOfflineService),
                Payload = forceDownload,
            };
        }

        private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

        private bool ShouldForceDownload => (bool)Payload;

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task RunApiServiceAsync()
        {
            await Task.Run(async () =>
            {
                List<Exception> exceptions = [];

                try
                {
                    await GetAllData(exceptions);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                if (exceptions.Count > 1)
                    throw new AggregateException(exceptions);
                else if (exceptions.Count > 0)
                    throw exceptions.First();
            });

            ResultCode = Result.Successful;
        }

        async Task GetAllData(List<Exception> exceptions)
        {
            // Synchronize both caseloads BEFORE getting any dependent info.
            // We don't want to start downloading dependent info before
            // caseload state is fully refreshed
            await Task.WhenAll(GetPersonalCaseload(), GetOfficeCaseload(exceptions));

            var cases = await GetRefreshableRecords<CaseRecord>();
            var incidents = await GetRefreshableRecords<IncidentRecord>();
            var memos = await GetRefreshableRecords<MemoRecord>();
            var srs = await GetRefreshableRecords<ServiceRequestRecord>();

            var casesIncidentsSrs = cases.Concat(incidents).Concat(srs);
            var memosIncidentsSrs = memos.Concat(incidents).Concat(srs);

            var all = casesIncidentsSrs.Concat(memos);

            await Task.WhenAll(
                GetAllNotes(casesIncidentsSrs, exceptions),
                GetAllVisits(cases, exceptions),
                GetAllContacts(all, exceptions),
                GetAllSupportNetworkItems(casesIncidentsSrs, exceptions),
                GetAllAttachments(all, exceptions),
                GetAllSafetyAssessments(incidents, exceptions),
                GetAllIncidentConcerns(incidents, exceptions),
                GetCallInformation(memosIncidentsSrs, exceptions),
                GetAllAdditionalInformation(memosIncidentsSrs, exceptions)
            );

            //Get Contact related info AFTER fetching all contacts from DBs
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            var allContacts = realm.All<IcmContact>().Freeze().ToList().Distinct();

            await Task.WhenAll(
                GetContactMedicalBehavioral(allContacts, exceptions),
                GetContactEducation(allContacts, exceptions)
            );

            // Get attachment files AFTER other dependent info so we
            // complete text-only downloads sooner
            await GetPartialAttachments(all, exceptions);
        }

        static async Task<IEnumerable<RecordServiceInfo>> GetRefreshableRecords<T>()
            where T : IBusinessObject
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            return realm
                .All<T>()
                .AsEnumerable()
                .Where(bo => bo.LocalState.ShouldDownloadDuringRefresh)
                .Select(bo => new RecordServiceInfo(bo))
                .ToList();
        }

        private async Task GetPersonalCaseload()
        {
            var caseloadMessage = GetCaseloadService.MakeStartMessage(ShouldForceDownload);
            await ServiceHandler.TryRunServiceAsync(caseloadMessage);
        }

        private async Task GetOfficeCaseload(List<Exception> exceptions)
        {
            try
            {
                var officeCaseloadMessage = GetOfficeCaseloadService.MakeStartMessage(ShouldForceDownload);
                await ServiceHandler.TryRunServiceAsync(officeCaseloadMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.Notes, ex));
            }
        }

        private static Exception MakeDownloadEx(string kind, Exception ex)
        {
            var msg = string.Format(LocalizedStrings.CaseloadErrorDownload, kind.ToLower());

            return new(msg, ex);
        }

        private async Task GetAllNotes(IEnumerable<RecordServiceInfo> casesIncidentsSrs, List<Exception> exceptions)
        {
            try
            {
                var allIdEntities = casesIncidentsSrs.Select(item => (item.FileNumber, item.Type));

                var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.Notes, ex));
            }
        }

        private async Task GetAllVisits(IEnumerable<RecordServiceInfo> cases, List<Exception> exceptions)
        {
            try
            {
                var allCaseIds = cases
                    .Where(@case => @case.Subtype == EntitySubtype.ChildServices)
                    .Select(@case => @case.Id);

                var startMessage = GetVisitsByRangeService.MakeStartMessage(allCaseIds);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.ChildYouthVisits, ex));
            }
        }

        private async Task GetAllContacts(IEnumerable<RecordServiceInfo> all, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetContactsByRangeService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.FamilyMembers, ex));
            }
        }

        private async Task GetAllSupportNetworkItems(
            IEnumerable<RecordServiceInfo> casesIncidentsSrs,
            List<Exception> exceptions
        )
        {
            try
            {
                var startMessage = GetSupportNetworkByRangeService.MakeStartMessage(casesIncidentsSrs);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.SupportNetwork, ex));
            }
        }

        private async Task GetAllAttachments(IEnumerable<RecordServiceInfo> all, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetAttachmentsByRangeService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.AttachmentMetadata, ex));
            }
        }

        private async Task GetPartialAttachments(IEnumerable<RecordServiceInfo> all, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetPartialAttachmentsByRangeDownloadService.MakeStartMessage(all);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.Attachments, ex));
            }
        }

        private async Task GetAllSafetyAssessments(IEnumerable<RecordServiceInfo> incidents, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetSafetyAssessmentsByRangeService.MakeStartMessage(incidents);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.SafetyAssessments, ex));
            }
        }

        private async Task GetAllIncidentConcerns(IEnumerable<RecordServiceInfo> incidents, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetIncidentConcernsByRangeService.MakeStartMessage(incidents);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.IncidentConcern, ex));
            }
        }

        private async Task GetCallInformation(
            IEnumerable<RecordServiceInfo> callInformation,
            List<Exception> exceptions
        )
        {
            try
            {
                var startMessage = GetCallInformationByRangeService.MakeStartMessage(callInformation);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.CallInformation, ex));
            }
        }

        private async Task GetAllAdditionalInformation(
            IEnumerable<RecordServiceInfo> incidentsMemosSrs,
            List<Exception> exceptions
        )
        {
            try
            {
                var startMessage = GetAdditionalInformationByRangeService.MakeStartMessage(incidentsMemosSrs);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.AdditionalInformation, ex));
            }
        }

        private async Task GetContactMedicalBehavioral(IEnumerable<IcmContact> allContacts, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetContactMedicalBehavioralByRangeService.MakeStartMessage(allContacts);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.ContactMedicalBehavioral, ex));
            }
        }

        private async Task GetContactEducation(IEnumerable<IcmContact> allContacts, List<Exception> exceptions)
        {
            try
            {
                var startMessage = GetContactEducationByRangeService.MakeStartMessage(allContacts);
                await ServiceHandler.TryRunServiceAsync(startMessage);
            }
            catch (Exception ex)
            {
                exceptions.Add(MakeDownloadEx(LocalizedStrings.ContactEducation, ex));
            }
        }
    }
}
