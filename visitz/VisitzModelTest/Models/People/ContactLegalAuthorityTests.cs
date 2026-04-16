using VisitzApi.Models.People;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class ContactLegalAuthorityTests
{
    private static readonly List<ContactLegalAuthorityJson> legalAuthJson =
    [
        new()
        {
            Updated = "12/10/2025 13:50:02",
            DependantSequenceNumber = "",
            PrimaryBandId = "",
            ErrorDescription = "",
            IndigenousCommunitiePartyAgreement = "",
            ReasonforService = "",
            Id = "1",
            IndigenousCommunities = "",
            Name = "",
            InCare = "",
            Designate = "",
            UpdatedBy = "",
            ByAgreementFlag = "",
            TermsandConditions = "",
            NextHearingDate = "12/10/2025 13:50:02",
            ParentContactId = "",
            DirectorsAuthority = "",
            ExpiryDateRequired = "",
            CreatedBy = "",
            IncidentId = "",
            LegalAuthorityDescription = "",
            EffectiveDate = "12/10/2025 13:50:02",
            Agreementwith = "",
            Comments = "",
            CaseId = "",
            ExpiryDate = "12/10/2025 13:50:02",
            Created = "12/10/2025 13:50:02",
            LastHearingDate = "12/10/2025 13:50:02",
            UpdatedByName = "",
            IndigenousComments = "",
            Type = "",
            LegalAuthorityCodeDeletion = "",
            LegalAuthorityCode = "",
            IntegrationState = "",
            EffectiveLegalStatus = "",
            CreatedByName = "",
            ReasonforService4 = "",
            ReasonforService3 = "",
            ReasonforService2 = "",
        },
        new()
        {
            Updated = "12/10/2025 13:50:02",
            DependantSequenceNumber = "",
            PrimaryBandId = "",
            ErrorDescription = "",
            IndigenousCommunitiePartyAgreement = "",
            ReasonforService = "",
            Id = "2",
            IndigenousCommunities = "",
            Name = "",
            InCare = "",
            Designate = "",
            UpdatedBy = "",
            ByAgreementFlag = "",
            TermsandConditions = "",
            NextHearingDate = "12/10/2025 13:50:02",
            ParentContactId = "",
            DirectorsAuthority = "",
            ExpiryDateRequired = "",
            CreatedBy = "",
            IncidentId = "",
            LegalAuthorityDescription = "",
            EffectiveDate = "12/10/2025 13:50:02",
            Agreementwith = "",
            Comments = "",
            CaseId = "",
            ExpiryDate = "12/10/2025 13:50:02",
            Created = "12/10/2025 13:50:02",
            LastHearingDate = "12/10/2025 13:50:02",
            UpdatedByName = "",
            IndigenousComments = "",
            Type = "",
            LegalAuthorityCodeDeletion = "",
            LegalAuthorityCode = "",
            IntegrationState = "",
            EffectiveLegalStatus = "",
            CreatedByName = "",
            ReasonforService4 = "",
            ReasonforService3 = "",
            ReasonforService2 = "",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddLegalAuthInformationToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuthorityTests>();
        List<ContactLegalAuthorityJson> legalAuthInfo = legalAuthJson;

        var numberOfLegalAuthorityInformationBeforeInsertion = realm.All<ContactLegalAuthority>().Count();
        await ContactLegalAuthority.SynchronizeAsync(realm, legalAuthInfo, "123");

        var numberOfLegalAuthorityInformationAfterInsertion = realm.All<ContactLegalAuthority>().Count();

        Assert.Equal(0, numberOfLegalAuthorityInformationBeforeInsertion);
        Assert.Equal(2, numberOfLegalAuthorityInformationAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesLegalAuthInfoDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuthorityTests>();
        List<ContactLegalAuthorityJson> legalAuthInfo = [.. legalAuthJson];

        await ContactLegalAuthority.SynchronizeAsync(realm, legalAuthInfo, "123");
        var numberOfLegalAuthorityInfoBeforeDeletion = realm.All<ContactLegalAuthority>().Count();

        legalAuthInfo.RemoveAll(item => item.Id == "2");
        await ContactLegalAuthority.SynchronizeAsync(realm, legalAuthInfo, "123");

        var numberOfLegalAuthorityInfoAfterDeletion = realm.All<ContactLegalAuthority>().Count();
        var inc = realm.All<ContactLegalAuthority>().ToList();

        Assert.Equal(2, numberOfLegalAuthorityInfoBeforeDeletion);
        Assert.Equal(numberOfLegalAuthorityInfoBeforeDeletion - 1, numberOfLegalAuthorityInfoAfterDeletion);
    }
}
