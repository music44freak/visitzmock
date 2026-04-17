using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.CallDetails;
using Visitz.Services.Caseload;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.SafetyAssessments;
using Visitz.Services.Visits;
using VisitzModel.Storage;

namespace Visitz.VisitzConfig
{
    public static class VisitzServicesConfig
    {
        public static MauiAppBuilder ConfigureVisitzApiServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ServiceHandler>();

            builder.Services.AddTransient<GetCaseloadService>();
            builder.Services.AddTransient<GetNotesService>();
            builder.Services.AddTransient<GetNotesForRangeService>();
            builder.Services.AddTransient<GetAllDataForOfflineService>();
            builder.Services.AddTransient<GetAllDataForRecordService>();
            builder.Services.AddTransient<SubmitNoteService>();
            builder.Services.AddTransient<SubmitAndGetNotesService>();
            builder.Services.AddTransient<SubmitSafetyAssessmentService>();
            builder.Services.AddTransient<SubmitAttachmentService>();
            builder.Services.AddTransient<GetVisitsService>();
            builder.Services.AddTransient<GetVisitsByRangeService>();
            builder.Services.AddTransient<PostVisitService>();
            builder.Services.AddTransient<PostAndRefreshVisitService>();
            builder.Services.AddTransient<GetContactsService>();
            builder.Services.AddTransient<GetContactsByRangeService>();
            builder.Services.AddTransient<GetSupportNetworkService>();
            builder.Services.AddTransient<GetSupportNetworkByRangeService>();
            builder.Services.AddTransient<GetAttachmentsService>();
            builder.Services.AddTransient<GetAttachmentsByRangeService>();
            builder.Services.AddTransient<GetAttachmentContentService>();
            builder.Services.AddTransient<GetAttachmentContentByRangeService>();
            builder.Services.AddTransient<GetPartialAttachmentsByRangeDownloadService>();
            builder.Services.AddTransient<GetSafetyAssessmentsService>();
            builder.Services.AddTransient<GetSafetyAssessmentsByRangeService>();
            builder.Services.AddTransient<GetOfficeCaseloadService>();
            builder.Services.AddTransient<RecordCleanupService>();
            builder.Services.AddTransient<AutoRefreshService>();
            builder.Services.AddTransient<GetIncidentConcernsService>();
            builder.Services.AddTransient<GetIncidentConcernsByRangeService>();
            builder.Services.AddTransient<GetCallInformationService>();
            builder.Services.AddTransient<GetCallInformationByRangeService>();
            builder.Services.AddTransient<GetContactMedicalBehavioralService>();
            builder.Services.AddTransient<GetContactMedicalBehavioralByRangeService>();
            builder.Services.AddTransient<GetAdditionalInformationService>();
            builder.Services.AddTransient<GetAdditionalInformationByRangeService>();
            builder.Services.AddTransient<GetContactEducationService>();
            builder.Services.AddTransient<GetContactEducationByRangeService>();

            return builder;
        }

        public static MauiAppBuilder ConfigureVisitzUtilities(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton(_ => new LastUpdatedPrefs(Preferences.Default));
            builder.Services.AddSingleton(_ => new UserIgnoredContentPrefs(Preferences.Default));

            return builder;
        }
    }
}
