using System.Globalization;
using Realms;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class MemoRecord : IRealmObject, IRowMetadata, IBusinessObject, IAssignedMetadata, IApiJson<MemoJson>
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

    public EntityType EntityType => EntityType.Memo;

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public string DisplayAssignees => AssignedTo;

    public string Address { get; set; }

    public string AddressComments { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public DateTimeOffset? CallDate { get; set; }

    public DateTimeOffset? CallTime { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string CellPhone { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public string HomePhone { get; set; }

    public string MedicalExamRequired { get; set; }

    public string MemoType { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public DateTimeOffset? PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string RecordedBy { get; set; }

    public string Resolution { get; set; }

    public bool RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    public string TypeOfCaller { get; set; }

    public string Urgent { get; set; }

    private int SubtypeInt { get; set; } = (int)EntitySubtype.Screening;
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)SubtypeInt;
        set => SubtypeInt = (int)value;
    }

    public string EntitySubtypeInitials => EntitySubtype.GetDisplayInitials();

    public BoLocalState LocalState { get; set; }

    public string DisplayDate =>
        CallDate?.ToString(IBusinessObject.DisplayDateFormat, CultureInfo.InvariantCulture) ?? "";

    public string DisplayName => this.GetDisplayName();

    public string FullType => this.GetFullType();

    public IQueryable<IcmContact> Contacts => this.GetContacts();

    public MemoRecord() { }

    public MemoRecord(MemoJson json, BoLocalState localState = null)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        FileNumber = json.MemoNumber;
        GivenNames = json.GivenNames;
        LastName = json.LastName;
        AssignedTo = json.AssignedTo;
        AssignedToId = json.AssignedToId;
        Address = json.Address;
        AddressComments = json.AddressComments;
        AreAnyOfTheFamilyMembersIndigenous = json.AreAnyOfTheFamilyMembersIndigenous;
        CallDate = Timestamp.ParseDateTimeOffsetNullable(json.CallDate);
        CallTime = Timestamp.ParseDateTimeOffsetNullable(json.CallTime);
        CallerAddress = json.CallerAddress;
        CallerEmail = json.CallerEmail;
        CallerName = json.CallerName;
        CallerPhone = json.CallerPhone;
        CellPhone = json.CellPhone;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(json.ClosedDate);
        CreatedByOffice = json.CreatedByOffice;
        HomePhone = json.HomePhone;
        MedicalExamRequired = json.MedicalExamRequired;
        MemoType = json.MemoType;
        Method = json.Method;
        NatureOfCall = json.NatureOfCall;
        PccSummary = json.PccSummary;
        PoliceForce = json.PoliceForce;
        PoliceInvestigation = json.PoliceInvestigation;
        PoliceNotifiedDate = Timestamp.ParseDateTimeOffsetNullable(json.PoliceNotifiedDate);
        PoliceReportNumber = json.PoliceReportNumber;
        PreferredContactMethod = json.PreferredContactMethod;
        RecordedBy = json.RecordedBy;
        Resolution = json.Resolution;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        ServiceOffice = json.ServiceOffice;
        Status = json.Status;
        TypeOfCaller = json.TypeOfCaller;
        Urgent = json.Urgent;
    }

    public static List<MemoRecord> FromApiArray(IEnumerable<MemoJson> jsonArray)
    {
        List<MemoRecord> outList = [];

        foreach (var jsonItem in jsonArray)
            outList.Add(new MemoRecord(jsonItem));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        SectionJson<MemoJson> section,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
    {
        var currentAssignedIds = realm.All<MemoRecord>().AsEnumerable().Select(memo => memo.Id);
        var unassignedIds = currentAssignedIds.Except(section.AssignedIds);
        var memos = FromApiArray(section.Items ?? []);

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                CascadeDelete(realm, unassignedIds, userIgnoredPrefs);
                realm.Upsert(memos);
            }
        );
    }

    public void DeleteDependentData(
        UserIgnoredContentPrefs userIgnoredPrefs,
        Realm fromRealm = null,
        bool deleteLocalState = true
    )
    {
        fromRealm ??= Realm;

        IcmContact.RemoveByParent(fromRealm, EntityType.Memo, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.Memo, Id, userIgnoredPrefs);
        CallInformation.RemoveByParent(fromRealm, EntityType.Memo, Id);
        AdditionalInformation.RemoveByParent(fromRealm, EntityType.Memo, Id);
        ContactLegalAuditTrail.RemoveByParent(fromRealm, EntityType.Memo, Id);

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
        IEnumerable<string> unassignedIds,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
    {
        foreach (var id in unassignedIds)
            if (fromRealm.Find<MemoRecord>(id) is MemoRecord memo)
                memo.Delete(userIgnoredPrefs, fromRealm);
    }

    public MemoJson ToApiJson(string dateFormat = "s")
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
            MemoNumber = FileNumber,
            GivenNames = GivenNames,
            LastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            Address = Address,
            AddressComments = AddressComments,
            AreAnyOfTheFamilyMembersIndigenous = AreAnyOfTheFamilyMembersIndigenous,
            CallDate = CallDate?.ToString(dateFormat),
            CallTime = CallTime?.ToString(dateFormat),
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat),
            CreatedByOffice = CreatedByOffice,
            HomePhone = HomePhone,
            MedicalExamRequired = MedicalExamRequired,
            MemoType = MemoType,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PoliceForce = PoliceForce,
            PoliceInvestigation = PoliceInvestigation,
            PoliceNotifiedDate = PoliceNotifiedDate?.ToString(dateFormat),
            PoliceReportNumber = PoliceReportNumber,
            PreferredContactMethod = PreferredContactMethod,
            RecordedBy = RecordedBy,
            Resolution = Resolution,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            ServiceOffice = ServiceOffice,
            Status = Status,
            TypeOfCaller = TypeOfCaller,
            Urgent = Urgent,
        };
    }

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<MemoRecord>()
            .FirstOrDefault(memo =>
                memo.Id == draftItem.RelatedEntityId || memo.FileNumber == draftItem.RelatedEntityId
            );
    }

    public static IQueryable<MemoRecord> GetAllByAssignee(Realm realm, string username, bool invert = false)
    {
        string operation = invert ? "!=" : "==";

        return realm.All<MemoRecord>().Filter($"$0 {operation} {nameof(AssignedTo)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username;
    }
}
