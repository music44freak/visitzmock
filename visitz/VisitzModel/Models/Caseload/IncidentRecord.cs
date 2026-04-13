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
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Caseload;

public partial class IncidentRecord
    : IRealmObject,
        IRowMetadata,
        IBusinessObject,
        IAssignedMetadata,
        IApiJson<IncidentJson>
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

    public EntityType EntityType => EntityType.Incident;

    public string GivenNames { get; set; }

    public string LastName { get; set; }

    public string AssignedTo { get; set; }

    public string AssignedToId { get; set; }

    public IList<string> Assignees { get; }

    public string DisplayAssignees =>
        Assignees.Any()
            ? Assignees.Order().Aggregate((acc, assigned) => acc + Environment.NewLine + assigned)
            : AssignedTo;

    public string AddressComments { get; set; }

    public string Address { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string Caseload { get; set; }

    public string CellPhone { get; set; }

    public DateTimeOffset? ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public DateTimeOffset? DateReported { get; set; }

    public string HomePhone { get; set; }

    public string MedicalExamRequired { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public DateTimeOffset? PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string ProtectionResponse { get; set; }

    public string Resolution { get; set; }

    public string ResponsePriority { get; set; }

    public bool RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    private int TypeInt { get; set; }
    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)TypeInt;
        set => TypeInt = (int)value;
    }

    public string EntitySubtypeInitials => EntitySubtype.GetDisplayInitials();

    public string TypeOfCaller { get; set; }

    public BoLocalState LocalState { get; set; }

    public string DisplayDate =>
        DateReported?.ToString(IBusinessObject.DisplayDateFormat, CultureInfo.InvariantCulture) ?? "";

    public string DisplayName => this.GetDisplayName();

    public string FullType => this.GetFullType();

    public IQueryable<IcmContact> Contacts => this.GetContacts();

    public IncidentRecord() { }

    public IncidentRecord(IncidentJson json, string currentUsername = null)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        FileNumber = json.IncidentNumber;
        GivenNames = json.GivenNames;
        LastName = json.LastName;
        AssignedTo = json.AssignedTo;
        AssignedToId = json.AssignedToId;

        if (!string.IsNullOrWhiteSpace(AssignedTo) && !Assignees.Contains(AssignedTo))
            Assignees.Add(AssignedTo);

        if (!string.IsNullOrWhiteSpace(currentUsername) && !Assignees.Contains(currentUsername))
            Assignees.Add(currentUsername);

        AddressComments = json.AddressComments;
        Address = json.Address;
        AreAnyOfTheFamilyMembersIndigenous = json.AreAnyOfTheFamilyMembersIndigenous;
        CallerAddress = json.CallerAddress;
        CallerEmail = json.CallerEmail;
        CallerName = json.CallerName;
        CallerPhone = json.CallerPhone;
        Caseload = json.Caseload;
        CellPhone = json.CellPhone;
        ClosedDate = Timestamp.ParseDateTimeOffsetNullable(json.ClosedDate);
        CreatedByOffice = json.CreatedByOffice;
        DateReported = Timestamp.ParseDateTimeOffsetNullable(json.DateReported);
        HomePhone = json.HomePhone;
        MedicalExamRequired = json.MedicalExamRequired;
        Method = json.Method;
        NatureOfCall = json.NatureOfCall;
        PccSummary = json.PccSummary;
        PoliceForce = json.PoliceForce;
        PoliceInvestigation = json.PoliceInvestigation;
        PoliceNotifiedDate = Timestamp.ParseDateTimeOffsetNullable(json.PoliceNotifiedDate);
        PoliceReportNumber = json.PoliceReportNumber;
        PreferredContactMethod = json.PreferredContactMethod;
        ProtectionResponse = json.ProtectionResponse;
        Resolution = json.Resolution;
        ResponsePriority = json.ResponsePriority;
        RestrictedFlag = json.RestrictedFlag.ParseWordTruthiness();
        ServiceOffice = json.ServiceOffice;
        Status = json.Status;
        EntitySubtype = json.Type?.ParseEntitySubtype() ?? EntitySubtype.Unknown;
        TypeOfCaller = json.TypeOfCaller;
    }

    public IncidentJson ToApiJson(string dateFormat = "s")
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
            IncidentNumber = FileNumber,
            GivenNames = GivenNames,
            LastName = LastName,
            AssignedTo = AssignedTo,
            AssignedToId = AssignedToId,
            AddressComments = AddressComments,
            Address = Address,
            AreAnyOfTheFamilyMembersIndigenous = AreAnyOfTheFamilyMembersIndigenous,
            CallerAddress = CallerAddress,
            CallerEmail = CallerEmail,
            CallerName = CallerName,
            CallerPhone = CallerPhone,
            Caseload = Caseload,
            CellPhone = CellPhone,
            ClosedDate = ClosedDate?.ToString(dateFormat),
            CreatedByOffice = CreatedByOffice,
            DateReported = DateReported?.ToString(dateFormat),
            HomePhone = HomePhone,
            MedicalExamRequired = MedicalExamRequired,
            Method = Method,
            NatureOfCall = NatureOfCall,
            PccSummary = PccSummary,
            PoliceForce = PoliceForce,
            PoliceInvestigation = PoliceInvestigation,
            PoliceNotifiedDate = PoliceNotifiedDate?.ToString(dateFormat),
            PoliceReportNumber = PoliceReportNumber,
            PreferredContactMethod = PreferredContactMethod,
            ProtectionResponse = ProtectionResponse,
            Resolution = Resolution,
            ResponsePriority = ResponsePriority,
            RestrictedFlag = RestrictedFlag.AsTruthyChar(),
            ServiceOffice = ServiceOffice,
            Status = Status,
            Type = EntitySubtype.GetDisplayString(),
            TypeOfCaller = TypeOfCaller,
        };
    }

    public static List<IncidentRecord> FromApiJsonArray(
        IEnumerable<IncidentJson> jsonArray,
        string currentUsername = null
    )
    {
        List<IncidentRecord> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new IncidentRecord(jsonItem, currentUsername));

        return outList;
    }

    static IEnumerable<IncidentRecord> FilterUnsupportedSubtypes(IEnumerable<IncidentRecord> incidents)
    {
        return incidents.Where(incident => incident.EntitySubtype == EntitySubtype.ChildProtection);
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<IncidentRecord> newOfficeIncidents,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload
    )
    {
        if (newOfficeIncidents == null)
            return;

        bool isOfficeCaseload = !isPersonalCaseload;
        var incomingIncidents = FilterUnsupportedSubtypes(newOfficeIncidents);
        var currentAssigned = GetAllByAssignee(realm, currentUsername, isOfficeCaseload).ToList();
        var unassigned = currentAssigned.Except(incomingIncidents);

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                CascadeDelete(realm, unassigned, userIgnoredPrefs);
                foreach (var item in incomingIncidents)
                {
                    realm.Add(item, update: true);
                    item.UpsertLocalState(realm, markForDownload: isPersonalCaseload);
                }
            }
        );
    }

    public static Task SynchronizeAsync(
        Realm realm,
        IEnumerable<IncidentJson> newOfficeIncidents,
        UserIgnoredContentPrefs userIgnoredPrefs,
        string currentUsername,
        bool isPersonalCaseload
    )
    {
        return SynchronizeAsync(
            realm,
            FromApiJsonArray(newOfficeIncidents, currentUsername),
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

        NoteItem.RemoveByParentFileNumber(fromRealm, EntityType.Incident, FileNumber);
        IcmContact.RemoveByParent(fromRealm, EntityType.Incident, Id);
        SupportNetworkItem.RemoveByParent(fromRealm, EntityType.Incident, Id);
        Attachment.RemoveByParent(fromRealm, EntityType.Incident, Id, userIgnoredPrefs);
        AdditionalInformation.RemoveByParent(fromRealm, EntityType.Incident, Id);
        IncidentConcerns.RemoveByParent(fromRealm, Id);
        CallInformation.RemoveByParent(fromRealm, EntityType.Incident, Id);
        ContactLegalAuditTrail.RemoveByParent(fromRealm, EntityType.Incident, Id);

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
        IEnumerable<IncidentRecord> removeIncidents,
        UserIgnoredContentPrefs userIgnoredPrefs
    )
    {
        foreach (var incident in removeIncidents)
            incident.Delete(userIgnoredPrefs, fromRealm);
    }

    public static IBusinessObject GetByDraftItem(Realm realm, IDraftItem draftItem)
    {
        return realm
            .All<IncidentRecord>()
            .FirstOrDefault(incident =>
                incident.Id == draftItem.RelatedEntityId || incident.FileNumber == draftItem.RelatedEntityId
            );
    }

    public static IQueryable<IncidentRecord> GetAllByAssignee(Realm realm, string username, bool invert = false)
    {
        string operation = invert ? "NONE" : "ANY";

        return realm.All<IncidentRecord>().Filter($"$0 == {operation} {nameof(Assignees)}", username);
    }

    public bool IsAssigned(string username)
    {
        return AssignedTo == username || Assignees.Contains(username);
    }

    public bool Equals(IncidentRecord other)
    {
        return other != null && Id == other.Id && EntityType == other.EntityType;
    }

    public override bool Equals(object obj)
    {
        return obj is IncidentRecord info ? Equals(info) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        return EntityType.GetHashCode() * Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }
}
