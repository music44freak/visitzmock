using Visitz.Device;
using Visitz.Views;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;
using Visitz.Views.Drafts;
using Visitz.Views.Entity;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using Visitz.Views.Entity.SupportNetwork;
using Visitz.Views.Navigation;
using Visitz.Views.Root;
using Visitz.Views.Snackbar;
using Visitz.Views.Todo;
using Visitz.Views.User;
using Visitz.Views.WebViewer;

namespace Visitz.VisitzConfig;

public static class VisitzScreens
{
    public static MauiAppBuilder ConfigureVisitzScreens(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<RootPage>();
        builder.Services.AddSingleton<RootViewModel>();

        builder.Services.AddSingleton<NavRailViewModel>();

        builder.Services.AddTransient<VisitzSnackbar>();
        builder.Services.AddTransient<VisitzSnackbarViewModel>();

        builder.Services.AddTransient<DataRefreshButton>();
        builder.Services.AddTransient<DataRefreshViewModel>();

        builder.Services.AddSingleton<CaseloadContainerView>();

        builder.Services.AddSingleton<CaseloadView>();
        builder.Services.AddSingleton<CaseloadViewModel>();

        builder.Services.AddTransient<DeviceAuthenticator>();
        builder.Services.AddTransient<AppLockPage>();
        builder.Services.AddTransient<AppLockViewModel>();

        builder.Services.AddTransient<WebViewPage>();
        builder.Services.AddTransient<WebViewModel>();
        builder.Services.AddTransient<PdfDetailsView>();
        builder.Services.AddTransient<PdfDetailsViewModel>();

        builder.Services.AddTransient<EntityView>();
        builder.Services.AddTransient<EntityViewModel>();

        builder.Services.AddTransient<EntityDetailsView>();
        builder.Services.AddTransient<EntityDetailsViewModel>();

        builder.Services.AddTransient<EntityContactsView>();
        builder.Services.AddTransient<EntityContactsViewModel>();
        builder.Services.AddTransient<ContactItemView>();
        builder.Services.AddTransient<ContactItemViewModel>();

        builder.Services.AddTransient<EntityNotesView>();
        builder.Services.AddTransient<EntityNotesViewModel>();

        builder.Services.AddTransient<AttachmentsView>();
        builder.Services.AddTransient<AttachmentsViewModel>();

        builder.Services.AddTransient<AttachmentDraftsListView>();
        builder.Services.AddTransient<AttachmentDraftsListViewModel>();

        builder.Services.AddTransient<AttachmentsListView>();
        builder.Services.AddTransient<AttachmentsListViewModel>();

        builder.Services.AddTransient<TakePhotoView>();
        builder.Services.AddTransient<TakePhotoViewModel>();

        builder.Services.AddTransient<PhotoDetailsView>();
        builder.Services.AddTransient<PhotoDetailsViewModel>();

        builder.Services.AddTransient<SafetyAssessmentListView>();
        builder.Services.AddTransient<SafetyAssessmentListViewModel>();
        builder.Services.AddTransient<SafetyAssessmentEditView>();
        builder.Services.AddTransient<SafetyAssessmentEditViewModel>();

        builder.Services.AddTransient<ChildYouthVisitListView>();
        builder.Services.AddTransient<ChildYouthVisitListViewModel>();

        builder.Services.AddTransient<PublishPage>();
        builder.Services.AddTransient<NotePublishViewModel>();
        builder.Services.AddTransient<SafetyAssessmentPublishViewModel>();
        builder.Services.AddTransient<AttachmentDraftPublishViewModel>();
        builder.Services.AddTransient<ChildYouthVisitPublishViewModel>();

        builder.Services.AddTransient<NoteEntryView>();
        builder.Services.AddTransient<NoteEntryViewModel>();

        builder.Services.AddTransient<DebugOptionsPage>();
        builder.Services.AddTransient<DebugOptionsView>();
        builder.Services.AddTransient<DebugOptionsViewModel>();

        builder.Services.AddTransient<SessionPage>();
        builder.Services.AddTransient<SessionViewModel>();
        builder.Services.AddTransient<UserView>();
        builder.Services.AddTransient<UserViewModel>();

        builder.Services.AddTransient<CollectionNoticeView>();

        builder.Services.AddSingleton<DraftsContainerView>();
        builder.Services.AddSingleton<DraftsContainerViewModel>();

        builder.Services.AddTransient<DraftsList>();
        builder.Services.AddTransient<DraftsListViewModel>();

        builder.Services.AddTransient<ChildYouthVisitView>();
        builder.Services.AddTransient<ChildYouthVisitViewModel>();

        builder.Services.AddTransient<SupportNetworkListView>();
        builder.Services.AddTransient<SupportNetworkListViewModel>();

        builder.Services.AddTransient<TabView>();
        builder.Services.AddTransient<TabViewModel>();
        builder.Services.AddTransient<TabItemView>();

        builder.Services.AddSingleton<TodoContainerView>();
        builder.Services.AddSingleton<TodoContainerViewModel>();

        builder.Services.AddSingleton<TodoListView>();
        builder.Services.AddSingleton<TodoListViewModel>();

        builder.Services.AddTransient<NavDrawerContentView>();
        builder.Services.AddTransient<NavDrawerContentViewModel>();

        return builder;
    }
}
