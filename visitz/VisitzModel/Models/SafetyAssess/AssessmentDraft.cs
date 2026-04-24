using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class AssessmentDraft : IRealmObject, IDraftItem
{
    [PrimaryKey]
    public string DraftEntityId { get; set; } = Guid.NewGuid().ToString();

    public string RelatedEntityId
    {
        get => DraftEntityId;
        set { }
    }

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string Preview => GeneralStrings.SafetyAssessment;

    public string DraftLocation { get; set; } = string.Empty;

    int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;
    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;
    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    private bool disposedValue;
    private bool? relatedEntityAvailable;
    private bool? relatedEntityDownloaded;

    [Ignored]
    public Realm? RelatedEntityRealm { get; set; }

    [Ignored]
    public IQueryable<IBusinessObject>? RelatedEntitySubscriptionQuery { get; set; }

    [Ignored]
    public IDisposable? RelatedEntitySubscriptionToken { get; set; }

    /// <summary>
    /// Whether or not the related entity is available for the app to interact
    /// with at all.
    /// </summary>
    [Ignored]
    public bool? RelatedEntityAvailable
    {
        get => relatedEntityAvailable;
        set
        {
            relatedEntityAvailable = value;
            RaisePropertyChanged(nameof(RelatedEntityAvailable));
        }
    }

    /// <summary>
    /// Whether or not the related entity's depdendent data has been
    /// downloaded (or marked for download).
    /// </summary>
    [Ignored]
    public bool? RelatedEntityDownloaded
    {
        get => relatedEntityDownloaded;
        set
        {
            relatedEntityDownloaded = value;
            RaisePropertyChanged(nameof(RelatedEntityDownloaded));
        }
    }

    public static IQueryable<AssessmentDraft> GetAllByFileNumber(Realm realm, string fileNumber)
    {
        return realm.All<AssessmentDraft>().Where(d => d.DraftEntityId == fileNumber);
    }

    public static async Task<AssessmentDraft> Upsert(
        Realm realm,
        SafetyAssessment assessment,
        string draftLocation,
        EntityType type = EntityType.Incident,
        EntitySubtype subtype = EntitySubtype.ChildProtection
    )
    {
        var draft =
            realm.Find<AssessmentDraft>(assessment.IncidentNumber)
            ?? new() { DraftEntityId = assessment.IncidentNumber };

        await realm.WriteAsync(() =>
        {
            if (!assessment.IsManaged)
                realm.Add(assessment);

            draft.DraftLocation = draftLocation;
            draft.RelatedEntityType = type;
            draft.RelatedEntitySubtype = subtype;
            draft.LastUpdated = DateTimeOffset.Now;

            realm.Add(draft, update: true);
        });

        return draft;
    }

    public static async Task TryDeleteAsync(SafetyAssessment assessment)
    {
        if (assessment?.Realm == null)
            return;

        var realm = assessment.Realm;

        await realm.WriteAsync(() =>
        {
            if (realm.Find<AssessmentDraft>(assessment.IncidentNumber) is AssessmentDraft draft)
                realm.Remove(draft);

            if (assessment.IsManaged)
                realm.Remove(assessment);
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                RelatedEntitySubscriptionToken?.Dispose();
                RelatedEntitySubscriptionToken = null;
                RelatedEntitySubscriptionQuery = null;
                RelatedEntityRealm = null;
                RelatedEntityAvailable = null;
                RelatedEntityDownloaded = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public int CompareTo(IDraftItem? other)
    {
        return this.CompareDraftItem(other);
    }
}
