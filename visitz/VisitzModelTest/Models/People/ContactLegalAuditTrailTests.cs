using VisitzApi.Models.People;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class ContactLegalAuditTrailTests
{
    private static readonly List<ContactLegalAuditTrailJson> legalAuditTrailCaseJsonData =
    [
        new()
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedbyName = "efewf",
            EmployeeLogin = "dsede",
            EntityId = "1-dferf",
            ID = "1",
            LegalAuthorityCode = "dewdew",
            OperationPerformed = "hdhddh",
            Type = "ert",
            Updated = "12/11/2025 13:50:02",
            Updatedby = "adsa",
            UpdatedByName = "hushs",
        },
        new()
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedbyName = "efewf",
            EmployeeLogin = "dsede",
            EntityId = "1-dferf",
            ID = "2",
            LegalAuthorityCode = "dewdew",
            OperationPerformed = "hdhddh",
            Type = "ert",
            Updated = "12/11/2025 13:50:02",
            Updatedby = "adsa",
            UpdatedByName = "hushs",
        },
    ];

    private static readonly List<ContactLegalAuditTrailJson> serviceRequestMedicalBehavioralJsons =
    [
        new()
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedbyName = "efewf",
            EmployeeLogin = "dsede",
            EntityId = "1-dferf",
            ID = "3",
            LegalAuthorityCode = "dewdew",
            OperationPerformed = "hdhddh",
            Type = "ert",
            Updated = "12/11/2025 13:50:02",
            Updatedby = "adsa",
            UpdatedByName = "hushs",
        },
        new()
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedbyName = "efewf",
            EmployeeLogin = "dsede",
            EntityId = "1-dferf",
            ID = "4",
            LegalAuthorityCode = "dewdew",
            OperationPerformed = "hdhddh",
            Type = "ert",
            Updated = "12/11/2025 13:50:02",
            Updatedby = "adsa",
            UpdatedByName = "hushs",
        },
        new()
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedbyName = "efewf",
            EmployeeLogin = "dsede",
            EntityId = "1-dferf",
            ID = "5",
            LegalAuthorityCode = "dewdew",
            OperationPerformed = "hdhddh",
            Type = "ert",
            Updated = "12/11/2025 13:50:02",
            Updatedby = "adsa",
            UpdatedByName = "hushs",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddContactLegalAuditTrailInfoForCasesToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuditTrailTests>();
        List<ContactLegalAuditTrailJson> contactMedicalBehavioral = legalAuditTrailCaseJsonData;

        var numberOfCaseContactMedicalBehavioralBeforeInsertion = realm.All<ContactLegalAuditTrail>().Count();
        await ContactLegalAuditTrail.SynchronizeAsync(realm, contactMedicalBehavioral, "10", EntityType.Case);

        var numberOfCaseContactMedicalBehavioralAfterInsertion = realm.All<ContactLegalAuditTrail>().Count();

        Assert.Equal(0, numberOfCaseContactMedicalBehavioralBeforeInsertion);
        Assert.Equal(2, numberOfCaseContactMedicalBehavioralAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesCasetLegalAuditTrailDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuditTrailTests>();
        List<ContactLegalAuditTrailJson> contactMedicalBehavioral = [.. legalAuditTrailCaseJsonData];
        string parentId = "10";

        await ContactLegalAuditTrail.SynchronizeAsync(realm, contactMedicalBehavioral, parentId, EntityType.Case);
        var numberOfCaseContactMedicalBehavioralBeforeDeletion = realm.All<ContactLegalAuditTrail>().Count();

        contactMedicalBehavioral.RemoveAll(item => item.ID == "1");

        await ContactLegalAuditTrail.SynchronizeAsync(realm, contactMedicalBehavioral, parentId, EntityType.Case);
        var numberOfCaseContactMedicalBehavioralAfterDeletion = realm.All<ContactLegalAuditTrail>().Count();

        Assert.Equal(2, numberOfCaseContactMedicalBehavioralBeforeDeletion);
        Assert.Equal(
            numberOfCaseContactMedicalBehavioralBeforeDeletion - 1,
            numberOfCaseContactMedicalBehavioralAfterDeletion
        );
    }

    [Fact]
    public async Task SynchronizeAsyncAddContacttLegalAuditTrailInfoToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuditTrailTests>();
        List<ContactLegalAuditTrailJson> contactMedicalBehavioral = serviceRequestMedicalBehavioralJsons;

        var numberOfCaseContactMedicalBehavioralBeforeInsertion = realm.All<ContactLegalAuditTrail>().Count();
        await ContactLegalAuditTrail.SynchronizeAsync(realm, contactMedicalBehavioral, "12", EntityType.ServiceRequest);

        var numberOfCaseContactMedicalBehavioralAfterInsertion = realm.All<ContactLegalAuditTrail>().Count();

        Assert.Equal(0, numberOfCaseContactMedicalBehavioralBeforeInsertion);
        Assert.Equal(3, numberOfCaseContactMedicalBehavioralAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesSrContacttLegalAuditTrailDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLegalAuditTrailTests>();
        List<ContactLegalAuditTrailJson> contactMedicalBehavioral = [.. serviceRequestMedicalBehavioralJsons];
        string parentId = "12";

        await ContactLegalAuditTrail.SynchronizeAsync(
            realm,
            contactMedicalBehavioral,
            parentId,
            EntityType.ServiceRequest
        );
        var numberOfCaseContactMedicalBehavioralBeforeDeletion = realm.All<ContactLegalAuditTrail>().Count();

        contactMedicalBehavioral.RemoveAll(item => item.ID == "5");

        await ContactLegalAuditTrail.SynchronizeAsync(
            realm,
            contactMedicalBehavioral,
            parentId,
            EntityType.ServiceRequest
        );
        var numberOfCaseContactMedicalBehavioralAfterDeletion = realm.All<ContactLegalAuditTrail>().Count();

        Assert.Equal(3, numberOfCaseContactMedicalBehavioralBeforeDeletion);
        Assert.Equal(
            numberOfCaseContactMedicalBehavioralBeforeDeletion - 1,
            numberOfCaseContactMedicalBehavioralAfterDeletion
        );
    }
}
