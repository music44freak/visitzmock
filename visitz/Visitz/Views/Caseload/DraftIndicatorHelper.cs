using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class DraftIndicatorHelper : ObservableObject, IDisposable
{
    readonly ObservableRealmQueryMap realmQueryMap = new();

    bool disposedValue;

    public Task InitTask { get; }

    [ObservableProperty]
    public HashSet<(string EntityId, EntityType Type)> draftedNotes = [];

    [ObservableProperty]
    public HashSet<(string EntityId, EntityType Type)> draftedAssessments = [];

    [ObservableProperty]
    public HashSet<(string EntityId, EntityType Type)> draftedAttachments = [];

    [ObservableProperty]
    public HashSet<(string EntityId, EntityType Type)> draftedVisits = [];

    [ObservableProperty]
    public HashSet<(string EntityId, EntityType Type)> draftedItems = [];

    public DraftIndicatorHelper()
    {
        realmQueryMap.ItemsChanged += RealmQueryMap_DraftsChanged;
        InitTask = InitAsync();
    }

    public async Task InitAsync()
    {
        var noteDraft = await VisitzRealms.GetNoteDraftsRealmAsync();
        realmQueryMap.Subscribe(noteDraft, noteDraft.All<NoteDraft>());

        var assessmentDraft = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        realmQueryMap.Subscribe(assessmentDraft, assessmentDraft.All<AssessmentDraft>());

        var attachmentDraft = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        realmQueryMap.Subscribe(attachmentDraft, attachmentDraft.All<AttachmentDraft>());

        var visitDraft = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        realmQueryMap.Subscribe(visitDraft, visitDraft.All<PersonVisitDraft>());
    }

    private void RealmQueryMap_DraftsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        HashSet<(string EntityId, EntityType Type)> drafted = [];

        foreach (var item in e.Items.Cast<IDraftItem>())
            drafted.Add((item.RelatedEntityId, item.RelatedEntityType));

        if (e.Type == typeof(NoteDraft))
            DraftedNotes = drafted;
        else if (e.Type == typeof(AssessmentDraft))
            DraftedAssessments = drafted;
        else if (e.Type == typeof(AttachmentDraft))
            DraftedAttachments = drafted;
        else if (e.Type == typeof(PersonVisitDraft))
            DraftedVisits = drafted;
    }

    partial void OnDraftedNotesChanged(HashSet<(string EntityId, EntityType Type)> value)
    {
        var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
        newSet.UnionWith(DraftedAssessments);
        newSet.UnionWith(DraftedAttachments);
        newSet.UnionWith(DraftedVisits);
        DraftedItems = newSet;
    }

    partial void OnDraftedAssessmentsChanged(HashSet<(string EntityId, EntityType Type)> value)
    {
        var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
        newSet.UnionWith(DraftedNotes);
        newSet.UnionWith(DraftedAttachments);
        newSet.UnionWith(DraftedVisits);
        DraftedItems = newSet;
    }

    partial void OnDraftedAttachmentsChanged(HashSet<(string EntityId, EntityType Type)> value)
    {
        var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
        newSet.UnionWith(DraftedNotes);
        newSet.UnionWith(DraftedAssessments);
        newSet.UnionWith(DraftedVisits);
        DraftedItems = newSet;
    }

    partial void OnDraftedVisitsChanged(HashSet<(string EntityId, EntityType Type)> value)
    {
        var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
        newSet.UnionWith(DraftedAssessments);
        newSet.UnionWith(DraftedAttachments);
        newSet.UnionWith(DraftedNotes);
        DraftedItems = newSet;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                realmQueryMap?.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
