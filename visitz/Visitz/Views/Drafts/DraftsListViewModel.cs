using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsListViewModel : VisitzViewModel
{
    bool _disposed;

    [ObservableProperty]
    public bool showEmptyView;

    [ObservableProperty]
    public ObservableCollection<IDraftItem> draftItems = [];

    ObservableCollection<AssessmentDraft> AssessmentDrafts { get; set; } = [];

    ObservableCollection<AttachmentDraft> AttachmentDrafts { get; set; } = [];

    ObservableCollection<NoteDraft> NoteDrafts { get; set; } = [];

    ObservableCollection<PersonVisitDraft> VisitDrafts { get; set; } = [];

    readonly ObservableRealmQueryMap queryMap = new();

    Realm? DataRealm { get; set; }

    EntitySection SectionToOpen { get; set; }

    public event EventHandler<IDraftItem>? SelectedItemRelatedMissing;

    // TODO: Use VisitzModel.Models.Drafts.MasterDraftItem to handle filtering the list

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        queryMap.ItemsChanged += QueryMap_ItemsChanged;
        await SubscribeForDrafts();
    }

    async Task SubscribeForDrafts()
    {
        Task<Realm> assessmentsTask = VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        Task<Realm> attachmentsTask = VisitzRealms.GetAttachmentDraftsRealmAsync();
        Task<Realm> notesTask = VisitzRealms.GetNoteDraftsRealmAsync();
        Task<Realm> visitsTask = VisitzRealms.GetPersonVisitDraftsRealmAsync();

        Realm assessmentsRealm = await assessmentsTask;
        Realm attachmentsRealm = await attachmentsTask;
        Realm notesRealm = await notesTask;
        Realm visitsRealm = await visitsTask;

        queryMap.Subscribe(assessmentsRealm, assessmentsRealm.All<AssessmentDraft>());
        AssessmentDrafts.CollectionChanged += DraftsLists_CollectionChanged;

        queryMap.Subscribe(attachmentsRealm, attachmentsRealm.All<AttachmentDraft>());
        AttachmentDrafts.CollectionChanged += DraftsLists_CollectionChanged;

        queryMap.Subscribe(notesRealm, notesRealm.All<NoteDraft>());
        NoteDrafts.CollectionChanged += DraftsLists_CollectionChanged;

        queryMap.Subscribe(visitsRealm, visitsRealm.All<PersonVisitDraft>());
        VisitDrafts.CollectionChanged += DraftsLists_CollectionChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            queryMap.ItemsChanged -= QueryMap_ItemsChanged;
            queryMap.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private void QueryMap_ItemsChanged(object? _, (Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e)
    {
        Type type = e.Item1;

        if (type == typeof(NoteDraft))
            UpdateSupportingList(e.Items, e.Changes, NoteDrafts);
        else if (type == typeof(AssessmentDraft))
            UpdateSupportingList(e.Items, e.Changes, AssessmentDrafts);
        else if (type == typeof(AttachmentDraft))
            UpdateSupportingList(e.Items, e.Changes, AttachmentDrafts);
        else if (type == typeof(PersonVisitDraft))
            UpdateSupportingList(e.Items, e.Changes, VisitDrafts);
        else
            throw new InvalidOperationException($"Type {type} not supported in Drafts view.");
    }

    static void UpdateSupportingList<T>(
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes,
        ObservableCollection<T> draftsList
    )
        where T : IDraftItem
    {
        if (changes == null)
        {
            foreach (var realmObj in items)
                draftsList.Add((T)realmObj);
        }
        else
        {
            foreach (int deleteIndex in changes.DeletedIndices.Reverse())
                draftsList.RemoveAt(deleteIndex);

            foreach (int insertIndex in changes.InsertedIndices)
                draftsList.Add((T)items.ElementAt(insertIndex));
        }
    }

    private void DraftsLists_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                IDraftItem draft = (IDraftItem)item;
                draft.SubscribeRelatedState(DataRealm);

                DraftItems.InsertSorted(draft, ascending: false);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                IDraftItem draft = (IDraftItem)item;
                draft.RelatedEntitySubscriptionToken?.Dispose();

                DraftItems.Remove((IDraftItem)item);
            }
        }
        else
        {
            Logger.LogInformation($"Unhandled CollectionChanged action: '{e.Action}'");
        }
    }

    [RelayCommand]
    private async Task DraftItemSelected(IDraftItem draftItem)
    {
        if (DataRealm == null)
        {
            Logger.LogWarning("Realm unexpectedly null");
            return;
        }

        Type type = draftItem.GetType();
        if (type == typeof(NoteDraft))
            SectionToOpen = EntitySection.NoteEntry;
        else if (type == typeof(AssessmentDraft))
            SectionToOpen = EntitySection.SafetyAssessmentEntry;
        else if (type == typeof(AttachmentDraft))
            SectionToOpen = EntitySection.Attachments;
        else if (type == typeof(PersonVisitDraft))
            SectionToOpen = EntitySection.ChildYouthVisitsEntry;

        if (draftItem.GetRelatedBusinessObjectFrom(DataRealm) is IBusinessObject bobj)
            await MarkForDownloadAndTryOpen(bobj, draftItem);
        else
            SelectedItemRelatedMissing?.Invoke(this, draftItem);
    }

    private async Task MarkForDownloadAndTryOpen(IBusinessObject bobj, IDraftItem draftItem)
    {
        bool markForDownload = !bobj.LocalState.ShouldDownloadDuringRefresh;

        if (markForDownload)
        {
            if (await bobj.PromptCanDownloadDependentData())
            {
                try
                {
                    await bobj.DownloadDependentData();
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
                }
            }
            // else: cancel
        }
        else
            NavigateTo(bobj, SectionToOpen, draftItem);
    }

    static void NavigateTo(IBusinessObject businessObject, EntitySection section, IDraftItem draftItem)
    {
        var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
        StrongReferenceMessenger.Default.Send(appNav);

        var caseloadNav = new BusinessObjectSelectedMessage(businessObject, section, draftItem);
        StrongReferenceMessenger.Default.Send(caseloadNav);
    }

    public async Task DeleteDraftAsync(IDraftItem draft)
    {
        var realm = draft.Realm;

        if (realm == null)
        {
            Logger.LogWarning("Realm unexpectedly null");
            return;
        }

        await realm.WriteAsync(async () =>
        {
            if (draft is AssessmentDraft)
            {
                var assessment = SafetyAssessment.FindByIncidentNumber(realm, draft.RelatedEntityId);

                if (assessment != null)
                    realm.Remove(assessment);

                realm.Remove(draft);
            }
            else if (draft is AttachmentDraft attachmentDraft)
                await attachmentDraft.Attachment.DeleteAsync();
            else
                realm.Remove(draft);
        });
    }
}
