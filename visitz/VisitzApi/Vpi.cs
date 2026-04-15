using VisitzApi.Endpoints;
using VisitzApi.Endpoints.Attachments;
using VisitzApi.Endpoints.CallDetails;
using VisitzApi.Endpoints.Caseload;
using VisitzApi.Endpoints.Notes;
using VisitzApi.Endpoints.People;
using VisitzApi.Endpoints.SafetyAssess;
using VisitzApi.Endpoints.Visits;
using VisitzApi.ErrorHandling;
using VisitzApi.Models;
using VisitzApi.Models.Attachments;
using VisitzApi.Models.CallDetails;
using VisitzApi.Models.Caseload;
using VisitzApi.Models.People;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi
{
    /// <summary>
    /// VPI - Visitz (A)PI - convenience wrapper class for interaction with Visitz' API endpoints.
    /// </summary>
    public class Vpi(HttpClient httpClient, string baseVisitzApiUrl)
    {
        internal static readonly string V1 = "v1";
        internal static readonly string V2 = "v2";

        private HttpClient HttpClient { get; } = httpClient;
        private string BaseVisitzApiUrl { get; } = baseVisitzApiUrl;

        private bool IsV1Endpoint<T>(VisitzBaseEndpoint<T> endpoint)
        {
            return endpoint.RequestUrl.StartsWith(BaseVisitzApiUrl.Trim('/') + "/" + V1);
        }

        private bool IsV2WorkflowEndpoint<T>(VisitzBaseEndpoint<T> endpoint)
        {
            return endpoint.RequestUrl.StartsWith(BaseVisitzApiUrl.Trim('/') + $"/{V2}/wf");
        }

        private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
        {
            var request = endpoint.MakeRequest();
            request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;

            var response = await HttpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            using ResponseBodyParser bodyParser = new(content);

            endpoint.ThrowOnHttpErrors(response, bodyParser);

            if (IsV2WorkflowEndpoint(endpoint) || IsV1Endpoint(endpoint))
                endpoint.ThrowOnErrorsInBody(response, bodyParser);

            return endpoint.HandleResponse(response, content);
        }

        public async Task<(int TotalRecords, CaseloadJson)> GetCaseloadAsync(Pagination pagination)
        {
            return await CallApi(new GetCaseloadEndpoint(BaseVisitzApiUrl, pagination));
        }

        public async Task<(int TotalRecords, OfficeCaseloadJson)> GetOfficeCaseloadAsync(Pagination? pagination = null)
        {
            return await CallApi(new GetOfficeCaseloadEndpoint(BaseVisitzApiUrl, pagination));
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            return await CallApi(new GetNotesEndpoint(BaseVisitzApiUrl, entityNumber, entityType));
        }

        public async Task<(bool success, string noteId)> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
        {
            return await CallApi(new SubmitNotesEndpoint(BaseVisitzApiUrl, noteToSubmit));
        }

        public async Task<(bool success, string status)> SubmitSafetyAssessmentAsync(
            SubmitSafetyAssessmentJson safetyAssessment
        )
        {
            return await CallApi(new SubmitSafetyAssessmentEndpoint(BaseVisitzApiUrl, safetyAssessment));
        }

        public async Task<(int TotalRecords, IEnumerable<VisitJson>)> GetVisitsAsync(
            string caseId,
            Pagination? pagination = null
        )
        {
            return await CallApi(new GetVisitsEndpoint(BaseVisitzApiUrl, caseId, pagination));
        }

        public async Task<bool> PostVisitAsync(string caseId, PostVisitJson visitJsonToSend)
        {
            return await CallApi(new PostVisitEndpoint(BaseVisitzApiUrl, caseId, visitJsonToSend));
        }

        public async Task<(int TotalRecords, IEnumerable<ContactJson>)> GetContactsAsync(
            ApiRecordType type,
            string id,
            Pagination? pagination = null
        )
        {
            return await CallApi(new GetContactsEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<SupportNetworkJson>)> GetSupportNetworkAsync(
            ApiRecordType type,
            string id,
            Pagination? pagination = null
        )
        {
            return await CallApi(new GetSupportNetworkEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<AttachmentJson>)> GetAttachmentsAsync(
            ApiRecordType type,
            string id,
            Pagination? pagination = null
        )
        {
            return await CallApi(new GetAttachmentsEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<AttachmentJson?> GetAttachmentDetailsAsync(
            ApiRecordType type,
            string recordId,
            string attachmentId,
            DateTimeOffset? after = null
        )
        {
            return await CallApi(
                new GetAttachmentDetailsEndpoint(BaseVisitzApiUrl, type, recordId, attachmentId, after)
            );
        }

        public async Task<(bool TotalCount, string AttachmentId)> SubmitAttachmentAsync(
            ApiRecordType type,
            string recordId,
            AttachmentFormData data
        )
        {
            return await CallApi(new PostAttachmentEndpoint(BaseVisitzApiUrl, type, recordId, data));
        }

        public async Task<(int TotalRecords, IEnumerable<GetSafetyAsessmentJson>)> GetSafetyAssessments(
            string incidentId,
            Pagination? pagination = null
        )
        {
            return await CallApi(new GetSafetyAssessmentsEndpoint(BaseVisitzApiUrl, incidentId, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<IncidentConcernsJson>)> GetIncidentConcerns(
            string incidentId,
            Pagination? pagination = null
        )
        {
            return await CallApi(new IncidentConcernsEndpoint(BaseVisitzApiUrl, incidentId, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<CallInformationJson>)> GetCallInformation(
            ApiRecordType type,
            string recordId,
            Pagination? pagination = null
        )
        {
            return await CallApi(new CallInformationEndpoint(BaseVisitzApiUrl, type, recordId, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<AdditionalInformationJson>)> GetAdditionalInformation(
            ApiRecordType type,
            string id,
            Pagination? pagination = null
        )
        {
            return await CallApi(new AdditionalInformationEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<ContactMedicalBehavioralJson>)> GetContactMedicalBehavioral(
            ApiRecordType type,
            string recordId,
            string contactId,
            Pagination? pagination = null
        )
        {
            return await CallApi(
                new ContactMedicalBehavioralEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination)
            );
        }

        public async Task<(int TotalRecords, IEnumerable<ContactLanguageJson>)> GetContactLanguageAsync(
            ApiRecordType type,
            string recordId,
            string contactId,
            Pagination? pagination = null
        )
        {
            return await CallApi(new ContactLanguagesEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination));
        }
    }
}
