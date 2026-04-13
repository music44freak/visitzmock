using System.Globalization;
using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class CaseRecord
    : IRealmObject,
        IRowMetadata,
        IBusinessObject,
        IAssignedMetadata,
        IApiJson<CaseJson>,
        IEquatable<CaseRecord>
{
    [PrimaryKey]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string FileNumber { get; set; }

    public EntityType EntityType => EntityType.Case;

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public IList<string> Assignees { get; }

    public string DisplayAssignees =>
        Assignees.Any()
            ? Assignees.Order().Aggregate((acc, assigned) => acc + Environment.NewLine + assigned)?.Trim()
            : AssignedTo;

    public string Caseload { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CloseReason { get; set; }

    public string EarlyOpenReason { get; set; }

    public string IntegrationState { get; set; }

    public string LegacyFileNumber { get; set; }

    public string MiddleName { get; set; }

    public bool MyFSFlag { get; set; }

    public string Name { get; set; }

    public string ServiceOffice { get; set; }

    public string Organization { get; set; }

    public string RegionName { get; set; }

    public DateTimeOffset? RenewReviewDate { get; set; }

    public DateTimeOffset? ReopenedDate { get; set; }

    public bool RestrictedFlag { get; set; }

    public string Status { get; set; }

    int TypeInt { get; set; }
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string EntitySubtypeInitials => EntitySubtype.GetDisplayInitials();

    public string WorkQueue { get; set; }

    public BoLocalState LocalState { get; set; }

    public string DisplayDate => CreatedDate.ToString(IBusinessObject.DisplayDateFormat, CultureInfo.InvariantCulture);

    public string DisplayName => this.GetDisplayName();

    public string FullType => this.GetFullType();

    public IQueryable<IcmContact> Contacts => this.GetContacts();

    public CaseRecord() { }

    public CaseRecord(CaseJson caseJson, string currentUsername = null)
    {
        Id = caseJson.Id;
        CreatedBy = caseJson.CreatedBy;
        CreatedById = caseJson.CreatedById;
        UpdatedBy = caseJson.UpdatedBy;
        UpdatedById = caseJson.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(caseJson.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(caseJson.UpdatedDate);
        FileNumber = caseJson.CaseNum;
        GivenNames = caseJson.SubjectContactFirstName;
        LastName = caseJson.SubjectContactLastName;
        AssignedTo = caseJson.AssignedTo;
        AssignedToId = caseJson.AssignedToId;

        if (caseJson.Position?.Count > 0)
            foreach (var position in caseJson.Position)
                Assignees.Add(position.SalesRep);

        if (!string.IsNullOrWhiteSpace(AssignedTo) && !Assignees.Contains(AssignedTo))
            Assignees.Add(AssignedTo);

        if (!string.IsNullOrWhiteSpace(currentUsername) && !Assignees.Contains(currentUsername))
            Assignees.Add(currentUsername);

        Caseload = caseJson.Caseload;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ClosedDate);
        CloseReason = caseJson.CloseReason;
        EarlyOpenReason = caseJson.EarlyOpenReason;
        IntegrationState = caseJson.IntegrationState;
        LegacyFileNumber = caseJson.LegacyFileNumber;
        MiddleName = caseJson.MiddleName;
        MyFSFlag = caseJson.MyFSFlag.ParseWordTruthiness();
        Name = caseJson.Name;
        ServiceOffice = caseJson.OfficeName;
        Organization = caseJson.Organization;
        RegionName = caseJson.RegionName;
        RenewReviewDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.RenewReviewDate);
        ReopenedDate = Timestamp.ParseDateTimeOffsetNullable(caseJson.ReopenedDate);
        RestrictedFlag = caseJson.RestrictedFlag.ParseWordTruthiness();
        Status = caseJson.Status;
        EntitySubtype = caseJson.Type.ParseEntitySubtype();
        WorkQueue = caseJson.WorkQueue;
    }

    public CaseJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            CreatedById = CreatedById,
            UpdatedBy = UpdatedBy,
            UpdatedById = UpdatedById,
            CreatedDate = CreatedDate.ToString(dateFormat),
            UpdatedDate = UpdatedDate.ToString(dateFormat),
            CaseNum = FileNumber,
            SubjectContactFirstName = GivenNames,
            SubjectContactLastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            Caseload = Caseload,
            ClosedDate = Timestamp.WriteDateTimeOffset(ClosedDate, dateFormat),
            CloseReason = CloseReason,
            EarlyOpenReason = EarlyOpenReason,
            IntegrationState = IntegrationState,
            LegacyFileNumber = LegacyFileNumber,
            MiddleName = MiddleName,
            MyFSFlag = MyFSFlag.AsTruthyChar(),
            Name = Name,
            OfficeName = ServiceOffice,
            Organization = Organization,
            RegionName = RegionName,
            RenewReviewDate = Timestamp.WriteDateTimeOffset(RenewReviewDate, dateFormat),
            ReopenedDate = Timestamp.WriteDateTimeOffset(ReopenedDate, dateFormat),
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            Status = Status,
            Type = EntitySubtype.GetDisplayString(),
            WorkQueue = WorkQueue,
        };
    }

    public static List<CaseRecord> FromApiJsonArray(IEnumerable<CaseJson> jsonArray, string currentUsername = null)
    {
        List<CaseRecord> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new CaseRecord(jsonItem, currentUsername));

        return outList;
    }

    static IEnumerable<CaseRecord> FilterUnsupportedSubtypes(IEnumerable<CaseRecord> cases)
    {
        return cases.Where(@case =>
            @case.EntitySubtype == EntitySubtype.ChildServices
            || @case.EntitySubtype == EntitySubtype.FamilyServices
            || @case.EntitySubtype == EntitySubtype.CysnFamilyServices
            || @case.EntitySubtype == EntitySubtype.Resource
        );
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<CaseRecord> newOfficeCases,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload
    )
    {
        if (newOfficeCases == null)
            return;

        bool isOfficeCaseload = !isPersonalCaseload;
        var incomingCases = FilterUnsupportedSubtypes(newOfficeCases);
        var currentAssigned = GetAllByAssignee(realm, currentUsername, isOfficeCaseload).ToList();
        var unassigned = currentAssigned.Except(incomingCases);

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                CascadeDelete(realm, unassigned, userIgnoredPrefs);
                foreach (var item in incomingCases)
                {
                    realm.Add(item, update: true);
                    item.UpsertLocalState(realm, markForDownload: isPersonalCaseload);
                }
            }
        );
    }

    public static Task SynchronizeAsync(
        Realm realm,
        IEnumerable<CaseJson> newAssignedCases,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload
    )
    {
        return SynchronizeAsync(
            realm,
            FromApiJsonArray(newAssignedCases, currentUsername),
            userIgnoredPrefs,
            currentUsername,
            isPersonalCaseload
        );
    }

    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm fromRealm = null,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;

        NoteItem.RemoveByParentFileNumber(fromRealm, EntityType.Case, FileNumber);
        PersonVisit.RemoveByParent(fromRealm, EntityType.Case, Id);
        IcmContact.RemoveByParent(fromRealm, EntityType.Case, Id);
        SupportNetworkItem.RemoveByParent(fromRealm, EntityType.Case, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.Case, Id, userIgnoredPrefs);
        ContactLegalAuditTrail.RemoveByParent(fromRealm, EntityType.Case, Id);

        if (deleteLocalState)
            fromRealm.Remove(LocalState);
    }

    public void Delete(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm fromRealm = null,
        bool cascade = true,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;

        if (cascade)
            DeleteDependentData(userIgnoredPrefs, fromRealm, deleteLocalState);

        fromRealm.Remove(this);
    }

    static void CascadeDelete(
        Realm fromRealm,
        IEnumerable<CaseRecord> unassigned,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
    {
        foreach (var @case in unassigned)
            @case.Delete(userIgnoredPrefs, fromRealm);
    }

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case =>
                @case.Id == draftItem.RelatedEntityId || @case.FileNumber == draftItem.RelatedEntityId
            );
    }

    public static IBusinessObject GetByPersonVisitItem(Realm realm, PersonVisit item)
    {
        return realm
            .All<CaseRecord>()
            .FirstOrDefault(@case => @case.Id == item.ParentId || @case.FileNumber == item.ParentId);
    }

    public static IQueryable<CaseRecord> GetAllByAssignee(Realm realm, string username, bool invert = false)
    {
        string operation = invert ? "NONE" : "ANY";

        return realm.All<CaseRecord>().Filter($"$0 == {operation} {nameof(Assignees)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username || Assignees.Contains(username);
    }

    public bool Equals(CaseRecord other)
    {
        return other != null && Id == other.Id && EntityType == other.EntityType;
    }

    public override bool Equals(object obj)
    {
        return obj is CaseRecord info ? Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        return EntityType.GetHashCode() * Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }
}
