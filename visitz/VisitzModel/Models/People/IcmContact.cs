using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.People;

public partial class IcmContact
    : IRealmObject,
        IRowMetadata,
        IApiJson<ContactJson>,
        IParentRecord,
        IEqualityComparer<IcmContact>
{
    public static readonly int KeyPlayerSortPosition = 0;
    public static readonly int ParentCaregiverSortPosition = 1;
    public static readonly int SubjectChildSortPosition = 2;
    public static readonly int OtherSortPosition = int.MaxValue;

    static readonly string KeyPlayer = "Key player";

    [PrimaryKey]
    public string LocalId { get; set; }

    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string ParentId { get; set; }

    private int ParentTypeInt { get; set; }

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string _921Agt { get; set; }

    public int ActiveAddresses { get; set; }

    public int Age { get; set; }

    public string AkaFirstName { get; set; }

    public string AkaLastName { get; set; }

    public string Alerts { get; set; }

    public string AutismFundingPaused { get; set; }

    public string BceIdUserName { get; set; }

    public string CanadianCitizen { get; set; }

    public string CellPhone { get; set; }

    public string Citizen { get; set; }

    public string Citizenship { get; set; }

    public string City { get; set; }

    public string CollaborateId { get; set; }

    public string Comments { get; set; }

    public string ConcernsOutcome { get; set; }

    public string CoordinationAgtCa { get; set; }

    public string Country { get; set; }

    public string CountryOfBirth { get; set; }

    public DateTimeOffset? CurrentStartDate { get; set; }

    public string Cysn { get; set; }

    public DateTimeOffset? DateOfBirth { get; set; }

    public DateTimeOffset? CitizenUpdatedDate { get; set; }

    public DateTimeOffset? CitizenshipUpdatedDate { get; set; }

    public string Deceased { get; set; }

    public DateTimeOffset? DeceasedDate { get; set; }

    public string EndDate { get; set; }

    public string FirstName { get; set; }

    public string Gender { get; set; }

    public string GivenNames { get; set; }

    public string HomePhone { get; set; }

    public string ImmigrationStatus { get; set; }

    public string ImmigrationStatusUpdated { get; set; }

    public string Indigenous { get; set; }

    public string IntegrationState { get; set; }

    public string InvestigationOutcomeSummary { get; set; }

    public string LastName { get; set; }

    public string LegacyDependentSequence { get; set; }

    public string LegalStatus { get; set; }

    public string MessagePhone { get; set; }

    public string MiddleNames { get; set; }

    public DateTimeOffset? OriginalStartDate { get; set; }

    public bool IsParentCaregiver { get; set; }

    public string PersonIdIcm { get; set; }

    public string PersonIdMis { get; set; }

    public bool ResponsibleForAllegedMaltreatment { get; set; }

    public string PersonalHealthNumber { get; set; }

    public string PersonalHealthNumberVerified { get; set; }

    public string PostalCode { get; set; }

    public string PotentialDuplicate { get; set; }

    public string PotentialDuplicateComments { get; set; }

    public string PreferredLanguage { get; set; }

    public string Primary { get; set; }

    public string PrimaryAddress { get; set; }

    public string PrimaryEmail { get; set; }

    public string ProjectCode { get; set; }

    public string Province { get; set; }

    public string PstScore { get; set; }

    public string Relationship { get; set; }

    public string Role { get; set; }

    public string RowId { get; set; }

    public string SaetPaused { get; set; }

    public string SocialInsuranceNumber { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public string StreetAddress { get; set; }

    public string StreetAddress2 { get; set; }

    public string Subject { get; set; }

    public bool IsSubjectChild { get; set; }

    public string Title { get; set; }

    public string UnitNumber { get; set; }

    public string WorkPhone { get; set; }

    public string FullDisplayName => string.Join(" ", FirstName, MiddleNames, LastName);

    public string DateOfBirthFormatted =>
        DateOfBirth?.ToString(IcmDateFormats.BasicTimestampShort, CultureInfo.InvariantCulture);

    public string HomePhoneFormatted => PhoneNumberFormatter.Format(HomePhone);

    public string CellPhoneFormatted => PhoneNumberFormatter.Format(CellPhone);

    public bool IsKeyPlayer => Relationship == KeyPlayer;

    public string DisplayCoordinationAgtCa => CoordinationAgtCa?.ExtendYOrN() ?? string.Empty;

    public string Display_921Agt => _921Agt?.ExtendYOrN() ?? string.Empty;

    public int SortPositionAsc
    {
        get
        {
            if (IsKeyPlayer)
                return KeyPlayerSortPosition;
            else if (IsParentCaregiver)
                return ParentCaregiverSortPosition;
            else if (IsSubjectChild)
                return SubjectChildSortPosition;
            else
                return OtherSortPosition;
        }
    }

    public IcmContact() { }

    public IcmContact(ContactJson json, string parentId, EntityType type)
    {
        LocalId = MakeLocalId(json.Id, parentId);
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        ParentId = parentId;
        ParentType = type;
        _921Agt = json._92_1AGT;
        ActiveAddresses = int.TryParse(json.ActiveAddresses, out int addresses) ? addresses : -1;
        AkaFirstName = json.AKAFirstName;
        AkaLastName = json.AKALastName;
        Alerts = json.Alerts;
        AutismFundingPaused = json.AutismFundingPaused;
        BceIdUserName = json.BCeIDUserName;
        CanadianCitizen = json.CanadianCitizen;
        CellPhone = json.CellPhone;
        Citizen = json.Citizen;
        Citizenship = json.Citizenship;
        City = json.City;
        CollaborateId = json.CollaborateID;
        Comments = json.Comments;
        ConcernsOutcome = json.ConcernsOutcome;
        CoordinationAgtCa = json.CoordinationAGTCA;
        Country = json.Country;
        CountryOfBirth = json.CountryofBirth;
        CurrentStartDate = Timestamp.ParseDateTimeOffsetNullable(json.CurrentStartDate);
        Cysn = json.CYSN;
        DateOfBirth = Timestamp.ParseDateTimeOffsetNullable(json.DateofBirth);
        CitizenUpdatedDate = Timestamp.ParseDateTimeOffsetNullable(json.DateUpdated_CitizenUpdatedDate);
        CitizenshipUpdatedDate = Timestamp.ParseDateTimeOffsetNullable(json.DateUpdated_CitizenshipUpdatedDate);
        Deceased = json.Deceased;
        DeceasedDate = Timestamp.ParseDateTimeOffsetNullable(json.DeceasedDate);
        EndDate = json.EndDate;
        FirstName = json.FirstName;
        Gender = json.Gender;
        GivenNames = json.GivenNames;
        HomePhone = json.HomePhone;
        ImmigrationStatus = json.ImmigrationStatus;
        ImmigrationStatusUpdated = json.ImmigrationStatusUpdated;
        Indigenous = json.Indigenous;
        IntegrationState = json.IntegrationState;
        InvestigationOutcomeSummary = json.InvestigationOutcomeSummary;
        LastName = json.LastName;
        LegacyDependentSequence = json.LegacyDependentSequence;
        LegalStatus = json.LegalStatus;
        MessagePhone = json.MessagePhone;
        MiddleNames = json.MiddleNames;
        OriginalStartDate = Timestamp.ParseDateTimeOffsetNullable(json.OriginalStartDate);
        IsParentCaregiver = json.Parent_Caregiver?.ParseWordTruthiness() ?? false;
        PersonIdIcm = json.PersonIDICM;
        PersonIdMis = json.PersonIDMIS;
        ResponsibleForAllegedMaltreatment =
            json.PersonResponsibleforAllegedMaltreatment?.ParseWordTruthiness() ?? false;
        PersonalHealthNumber = json.PHN;
        PersonalHealthNumberVerified = json.PHNVerified;
        PostalCode = json.PostalCode;
        PotentialDuplicate = json.PotentialDuplicate;
        PotentialDuplicateComments = json.PotentialDuplicateComments;
        PreferredLanguage = json.PreferredLanguage;
        Primary = json.Primary;
        PrimaryAddress = json.PrimaryAddress;
        PrimaryEmail = json.PrimaryEmail;
        ProjectCode = json.ProjectCode;
        Province = json.Prov;
        PstScore = json.PSTScore;
        Relationship = json.Relationship;
        Role = json.Role;
        RowId = json.RowId;
        SaetPaused = json.SAETPaused;
        SocialInsuranceNumber = json.SIN;
        StartDate = Timestamp.ParseDateTimeOffsetNullable(json.StartDate);
        StreetAddress = json.StreetAddress;
        StreetAddress2 = json.StreetAddress2;
        Subject = json.Subject;
        IsSubjectChild = json.SubjectChild.ParseWordTruthiness();
        Title = json.Title;
        UnitNumber = json.UnitNumber;
        WorkPhone = json.WorkPhone;

        Age = TryParseAge(json.Age, json.DateofBirth);
    }

    static int TryParseAge(string age, string dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(age) && !string.IsNullOrWhiteSpace(dateOfBirth))
            return (DateTimeOffset.UtcNow - DateTimeOffset.Parse(dateOfBirth)).Days / 365;
        else
            return int.TryParse(age, out int parsed) ? parsed : -1;
    }

    static string MakeLocalId(string contactId, string parentId)
    {
        return $"{contactId}|{parentId}";
    }

    public ContactJson ToApiJson(string dateFormat = "s")
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
            _92_1AGT = _921Agt,
            ActiveAddresses = ActiveAddresses.ToString(),
            Age = Age.ToString(),
            AKAFirstName = AkaFirstName,
            AKALastName = AkaLastName,
            Alerts = Alerts,
            AutismFundingPaused = AutismFundingPaused,
            BCeIDUserName = BceIdUserName,
            CanadianCitizen = CanadianCitizen,
            CellPhone = CellPhone,
            Citizen = Citizen,
            Citizenship = Citizenship,
            City = City,
            CollaborateID = CollaborateId,
            Comments = Comments,
            ConcernsOutcome = ConcernsOutcome,
            CoordinationAGTCA = CoordinationAgtCa,
            Country = Country,
            CountryofBirth = CountryOfBirth,
            CurrentStartDate = CurrentStartDate?.ToString(dateFormat),
            CYSN = Cysn,
            DateofBirth = DateOfBirth?.ToString(dateFormat),
            DateUpdated_CitizenUpdatedDate = CitizenUpdatedDate?.ToString(dateFormat),
            DateUpdated_CitizenshipUpdatedDate = CitizenshipUpdatedDate?.ToString(dateFormat),
            Deceased = Deceased,
            DeceasedDate = DeceasedDate?.ToString(dateFormat),
            EndDate = EndDate,
            FirstName = FirstName,
            Gender = Gender,
            GivenNames = GivenNames,
            HomePhone = HomePhone,
            ImmigrationStatus = ImmigrationStatus,
            ImmigrationStatusUpdated = ImmigrationStatusUpdated,
            Indigenous = Indigenous,
            IntegrationState = IntegrationState,
            InvestigationOutcomeSummary = InvestigationOutcomeSummary,
            LastName = LastName,
            LegacyDependentSequence = LegacyDependentSequence,
            LegalStatus = LegalStatus,
            MessagePhone = MessagePhone,
            MiddleNames = MiddleNames,
            OriginalStartDate = OriginalStartDate?.ToString(dateFormat),
            Parent_Caregiver = IsParentCaregiver.AsTruthyChar(),
            PersonIDICM = PersonIdIcm,
            PersonIDMIS = PersonIdMis,
            PersonResponsibleforAllegedMaltreatment = ResponsibleForAllegedMaltreatment.AsTruthyChar(),
            PHN = PersonalHealthNumber,
            PHNVerified = PersonalHealthNumberVerified,
            PostalCode = PostalCode,
            PotentialDuplicate = PotentialDuplicate,
            PotentialDuplicateComments = PotentialDuplicateComments,
            PreferredLanguage = PreferredLanguage,
            Primary = Primary,
            PrimaryAddress = PrimaryAddress,
            PrimaryEmail = PrimaryEmail,
            ProjectCode = ProjectCode,
            Prov = Province,
            PSTScore = PstScore,
            Relationship = Relationship,
            Role = Role,
            RowId = RowId,
            SAETPaused = SaetPaused,
            SIN = SocialInsuranceNumber,
            StartDate = StartDate?.ToString(dateFormat),
            StreetAddress = StreetAddress,
            StreetAddress2 = StreetAddress2,
            Subject = Subject,
            SubjectChild = IsSubjectChild.AsTruthyChar(),
            Title = Title,
            UnitNumber = UnitNumber,
            WorkPhone = WorkPhone,
        };
    }

    public static IEnumerable<IcmContact> FromApiArray(
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type
    )
    {
        List<IcmContact> outList = [];

        foreach (var contactJson in contacts)
            outList.Add(new IcmContact(contactJson, parentId, type));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type
    )
    {
        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                var incomingContacts = FromApiArray(contacts, parentId, type);
                var incomingContactsIds = incomingContacts.Select(item => item.Id);
                var allIcmContacts = realm
                    .All<IcmContact>()
                    .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type)
                    .ToList();
                var allIcmContactIds = allIcmContacts.Select(item => item.Id);

                var contactIdsToDelete = allIcmContactIds.Except(incomingContactsIds);
                var contactsToDelete = allIcmContacts.Where(item => contactIdsToDelete.Contains(item.Id));

                foreach (var item in contactsToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingContacts);
            }
        );
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var contacts = realm
            .All<IcmContact>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        foreach (var contact in contacts)
        {
            ContactMedicalBehavioral.RemoveByParent(realm, contact.Id);
            ContactEducation.RemoveByParent(realm, contact.Id);
        }

        realm.RemoveRange(contacts);
    }

    public static IQueryable<IcmContact> GetByParentObject(Realm realm, IBusinessObject businessObject)
    {
        return realm
            .All<IcmContact>()
            .Where(contact =>
                contact.ParentId == businessObject.Id && contact.ParentTypeInt == (int)businessObject.EntityType
            );
    }

    public static IcmContact GetKeyPlayerFor(Realm realm, IBusinessObject businessObject)
    {
        return GetByParentObject(realm, businessObject)
            .Where(contact => contact.Relationship == KeyPlayer)
            .FirstOrDefault();
    }

    public bool Equals(IcmContact x, IcmContact y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Id == y.Id;
    }

    public int GetHashCode([DisallowNull] IcmContact obj)
    {
        if (obj is null)
            return 0;

        return obj.Id.GetHashCode();
    }
}
