using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Realms;
using Syncfusion.Maui.Toolkit.TabView;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel : IcmRecordViewModel, IRecipient<ServiceStateMessage>
{
    ServiceHandler ServiceHandler { get; } = ServiceProvider.GetService<ServiceHandler>();

    string? _cacheRemovedDisplayName;

    public EntitySection? RequestedSection { get; set; }

    public IDraftItem? FocusedDraftItem { get; set; }

    readonly ObservableRealmQueryMap _queryMap = new();

    [ObservableProperty]
    public bool downloadActivity;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _cacheRemovedDisplayName = BusinessObject.DisplayName;
        BusinessObject.SubscribePropertyChanged(BusinessObject_PropertyChanged);
        WeakReferenceMessenger.Default.Register(this, GetAllDataForRecordService.MakeId(BusinessObject));

        try
        {
            BuildNavList();

            if (RequestedSection != null)
                SelectedTab = GetMappedNavItem(RequestedSection);

            SelectedTab ??= GetTabByType<EntityDetailsView>();

            UpdateDownloadActivity();

            ServiceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
            ServiceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

            UpdateLocalActivityTimestamp();

            await SetupDraftIndicatorObservers();
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DisposeTabViews();

            BusinessObject.UnsubscribePropertyChanged(BusinessObject_PropertyChanged);

            ServiceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;
            ServiceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }
        base.Dispose(disposing);
    }

    async void BusinessObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IBusinessObject bobj)
            return;

        if (e.PropertyName == nameof(bobj.IsValid) && !bobj.IsValid)
            await EntityUnassignedGoBack();
    }

    async Task EntityUnassignedGoBack()
    {
        GoBack();

        string typeString = EntityType.GetDisplayString();

        await Navigator.CurrentOpenPage.DisplayAlertAsync(
            string.Format(LocalizedStrings.RecordRemovedFromCaseload, typeString, _cacheRemovedDisplayName),
            string.Format(LocalizedStrings.RecordRemovedFromCaseloadDetails, typeString, _cacheRemovedDisplayName),
            LocalizedStrings.Ok
        );
    }

    void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceFinished(object? sender, VisitzService e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void UpdateDownloadActivity()
    {
        DownloadActivity = BusinessObject.IsValid && ServiceHandler.IsAnyServiceRunning(BusinessObject.Id);
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.FinishedError)
        {
            string displayString = $"{EntityType.GetDisplayString()} {_cacheRemovedDisplayName}";
            string msg = string.Format(LocalizedStrings.DownloadRecordErrorMessage, displayString);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                msg,
                message.UncaughtException?.ToString() ?? string.Empty,
                LocalizedStrings.DownloadError
            );
        }
    }

    void UpdateLocalActivityTimestamp()
    {
        if (BusinessObject.IsValid)
            BusinessObject.LocalState.LastOpenedBinding = DateTimeOffset.UtcNow;
    }

    async Task SetupDraftIndicatorObservers()
    {
        string fileNumber = BusinessObject.FileNumber;
        _queryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        if (GetTabByType<EntityNotesView>() != null)
        {
            var noteRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
            _queryMap.Subscribe(
                noteRealm,
                noteRealm.All<NoteDraft>().Where(draft => draft.ParentEntityId == fileNumber)
            );
        }

        if (GetTabByType<AttachmentsView>() != null)
        {
            var attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
            _queryMap.Subscribe(
                attachmentsRealm,
                attachmentsRealm.All<AttachmentDraft>().Where(draft => draft.RelatedEntityId == fileNumber)
            );
        }

        if (GetTabByType<SafetyAssessmentListView>() != null)
        {
            var assessmentRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
            _queryMap.Subscribe(
                assessmentRealm,
                assessmentRealm.All<AssessmentDraft>().Where(draft => draft.DraftEntityId == fileNumber)
            );
        }

        if (GetTabByType<ChildYouthVisitListView>() != null)
        {
            var visitsRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
            _queryMap.Subscribe(
                visitsRealm,
                visitsRealm.All<PersonVisitDraft>().Where(draft => draft.RelatedEntityId == RowId)
            );
        }
    }

    void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(NoteDraft))
            TrySetDraftIndicator<EntityNotesView>(e.Items.Any());
        else if (e.Type == typeof(AssessmentDraft))
            TrySetDraftIndicator<SafetyAssessmentListView>(e.Items.Any());
        else if (e.Type == typeof(AttachmentDraft))
            TrySetDraftIndicator<AttachmentsView>(e.Items.Any());
        else if (e.Type == typeof(PersonVisitDraft))
            TrySetDraftIndicator<ChildYouthVisitListView>(e.Items.Any());
    }

    void TrySetDraftIndicator<T>(bool hasDraft)
        where T : BaseContentView
    {
        if (GetTabByType<T>() is not SfTabItem tab)
            return;

#pragma warning disable CS8601 // Possible null reference assignment.
        // SfTabView.ImageSource is not declared nullable even though documentation suggests it should
        // TODO: Remove suppression once fixed in library
        tab.ImageSource = hasDraft
            ? MaterialIcons.GetFilledMaterialIcon(MaterialIcons.Draft, VisitzColors.BC_Gold)
            : null;
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}
