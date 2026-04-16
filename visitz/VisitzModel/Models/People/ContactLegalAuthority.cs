using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

#nullable enable

namespace VisitzModel.Models.People;

public partial class ContactLegalAuthority : IRealmObject, IApiJson<ContactLegalAuthorityJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public string DependantSequenceNumber { get; set; } = string.Empty;
    public string PrimaryBandId { get; set; } = string.Empty;
    public string ErrorDescription { get; set; } = string.Empty;
    public string IndigenousCommunitiePartyAgreement { get; set; } = string.Empty;
    public string ReasonforService { get; set; } = string.Empty;
    public string IndigenousCommunities { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string InCare { get; set; } = string.Empty;
    public string Designate { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string ByAgreementFlag { get; set; } = string.Empty;
    public string TermsandConditions { get; set; } = string.Empty;
    public DateTimeOffset? NextHearingDate { get; set; }
    public string ParentContactId { get; set; } = string.Empty;
    public string DirectorsAuthority { get; set; } = string.Empty;
    public string ExpiryDateRequired { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LegalAuthorityDescription { get; set; } = string.Empty;
    public DateTimeOffset? EffectiveDate { get; set; }
    public string Agreementwith { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTimeOffset? ExpiryDate { get; set; }
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastHearingDate { get; set; }
    public string UpdatedByName { get; set; } = string.Empty;
    public string IndigenousComments { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string LegalAuthorityCodeDeletion { get; set; } = string.Empty;
    public string LegalAuthorityCode { get; set; } = string.Empty;
    public string IntegrationState { get; set; } = string.Empty;
    public string EffectiveLegalStatus { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string ReasonforService4 { get; set; } = string.Empty;
    public string ReasonforService3 { get; set; } = string.Empty;
    public string ReasonforService2 { get; set; } = string.Empty;

    public ContactLegalAuthority() { }

    public ContactLegalAuthority(ContactLegalAuthorityJson json, string parentContactId)
    {
        Id = json.Id;
        Updated = DateTimeOffset.Parse(json.Updated);
        DependantSequenceNumber = json.DependantSequenceNumber;
        PrimaryBandId = json.PrimaryBandId;
        ErrorDescription = json.ErrorDescription;
        IndigenousComments = json.IndigenousComments;
        IndigenousCommunitiePartyAgreement = json.IndigenousCommunitiePartyAgreement;
        IndigenousCommunities = json.IndigenousCommunities;
        ReasonforService = json.ReasonforService;
        Name = json.Name;
        InCare = json.InCare;
        Designate = json.Designate;
        UpdatedBy = json.UpdatedBy;
        ByAgreementFlag = json.ByAgreementFlag;
        TermsandConditions = json.TermsandConditions;
        NextHearingDate = Timestamp.ParseDateTimeOffsetNullable(json.NextHearingDate);
        ParentContactId = parentContactId;
        DirectorsAuthority = json.DirectorsAuthority;
        ExpiryDateRequired = json.ExpiryDateRequired;
        CreatedBy = json.CreatedBy;
        LegalAuthorityDescription = json.LegalAuthorityDescription;
        EffectiveDate = Timestamp.ParseDateTimeOffsetNullable(json.EffectiveDate);
        Agreementwith = json.Agreementwith;
        Comments = json.Comments;
        ExpiryDate = Timestamp.ParseDateTimeOffsetNullable(json.ExpiryDate);
        Created = DateTimeOffset.Parse(json.Created);
        LastHearingDate = Timestamp.ParseDateTimeOffsetNullable(json.LastHearingDate);
        UpdatedByName = json.UpdatedByName;
        Type = json.Type;
        LegalAuthorityCodeDeletion = json.LegalAuthorityCodeDeletion;
        LegalAuthorityCode = json.LegalAuthorityCode;
        IntegrationState = json.IntegrationState;
        EffectiveLegalStatus = json.EffectiveLegalStatus;
        CreatedByName = json.CreatedByName;
        ReasonforService2 = json.ReasonforService2;
        ReasonforService3 = json.ReasonforService3;
        ReasonforService4 = json.ReasonforService4;
    }

    public ContactLegalAuthorityJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Agreementwith = Agreementwith,
            ReasonforService4 = ReasonforService4,
            ReasonforService3 = ReasonforService3,
            ReasonforService2 = ReasonforService2,
            ReasonforService = ReasonforService,
            ByAgreementFlag = ByAgreementFlag,
            Comments = Comments,
            CreatedBy = CreatedBy,
            Created = Created.ToString(dateFormat) ?? string.Empty,
            CreatedByName = CreatedByName,
            DependantSequenceNumber = DependantSequenceNumber,
            Designate = Designate,
            DirectorsAuthority = DirectorsAuthority,
            EffectiveDate = EffectiveDate?.ToString(dateFormat) ?? string.Empty,
            EffectiveLegalStatus = EffectiveLegalStatus,
            ErrorDescription = ErrorDescription,
            ExpiryDate = ExpiryDate?.ToString(dateFormat) ?? string.Empty,
            ExpiryDateRequired = ExpiryDateRequired,
            Id = Id,
            InCare = InCare,
            IndigenousComments = IndigenousComments,
            IndigenousCommunitiePartyAgreement = IndigenousCommunitiePartyAgreement,
            IndigenousCommunities = IndigenousCommunities,
            IntegrationState = IntegrationState,
            LastHearingDate = LastHearingDate?.ToString(dateFormat) ?? string.Empty,
            LegalAuthorityCode = LegalAuthorityCode,
            LegalAuthorityCodeDeletion = LegalAuthorityCodeDeletion,
            LegalAuthorityDescription = LegalAuthorityDescription,
            Name = Name,
            NextHearingDate = NextHearingDate?.ToString(dateFormat) ?? string.Empty,
            ParentContactId = ParentContactId,
            PrimaryBandId = PrimaryBandId,
            TermsandConditions = TermsandConditions,
            Type = Type,
            Updated = Updated.ToString(dateFormat) ?? string.Empty,
            UpdatedBy = UpdatedBy,
            UpdatedByName = UpdatedByName,
        };
    }

    public static List<ContactLegalAuthority> FromApiJsonArray(
        IEnumerable<ContactLegalAuthorityJson> jsonArray,
        string parentContactId
    )
    {
        List<ContactLegalAuthority> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactLegalAuthority(jsonItem, parentContactId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactLegalAuthorityJson> contactLegalAuthority,
        string parentContactId
    )
    {
        if (contactLegalAuthority == null)
            return;

        var incomingContactLegalAuthority = FromApiJsonArray(contactLegalAuthority, parentContactId);
        var incomingContactLegalAuthorityIds = incomingContactLegalAuthority.Select(item => item.Id);

        var allContactLegalAuthority = realm
            .All<ContactLegalAuthority>()
            .Where(item => item.ParentContactId == parentContactId);
        var allContactLegalAuthorityIds = allContactLegalAuthority.AsEnumerable().Select(item => item.Id);

        var contactLegalAuthorityIdsToDelete = allContactLegalAuthorityIds.Except(incomingContactLegalAuthorityIds);
        var contactLegalAuthorityToDelete = allContactLegalAuthority
            .ToList()
            .Where(item => contactLegalAuthorityIdsToDelete.Contains(item.Id));

        if (!contactLegalAuthorityIdsToDelete.Any() && !incomingContactLegalAuthorityIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactLegalAuthorityToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingContactLegalAuthority);
            }
        );
    }

    public static void RemoveByParent(Realm realm, string parentContactId)
    {
        var contacts = realm.All<IcmContact>().Where(item => item.Id == parentContactId);

        if (contacts.Count() <= 1)
        {
            var contactLegalAuthorityToBeDeleted = realm
                .All<ContactLegalAuthority>()
                .Where(item => item.ParentContactId == parentContactId);

            realm.RemoveRange(contactLegalAuthorityToBeDeleted);
        }
    }
}
