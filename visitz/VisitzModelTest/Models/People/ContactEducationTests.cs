using VisitzApi.Models.People;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class ContactEducationTests
{
    private static readonly List<ContactEducationJson> EducationJsons =
    [
        new()
        {
            LearningAssistant = "dshec",
            ContactPerson = "cddc",
            SchoolName = "cdcd",
            IndividualEducationPlan = "dcdd",
            Comments = "cddcd",
            InstitutionId = "123",
            CreatedByName = "cdcsdc",
            EndDate = "12/11/2025 13:50:02",
            InstitutionName = "hhh",
            Degree = "mnn",
            ContactPersonRole = "hh",
            Id = "1",
            Year = "12/11/2025 13:50:02",
            Address = "cddsc",
            University = "cdsc",
            UpdatedByName = "cdsc",
            ContactId = "90217",
            StartDate = "12/11/2025 13:50:02",
            PhoneNum = "12345678",
            DateLastAttended = "12/11/2025 13:50:02",
        },
        new()
        {
            LearningAssistant = "dshec",
            ContactPerson = "cddc",
            SchoolName = "cdcd",
            IndividualEducationPlan = "dcdd",
            Comments = "cddcd",
            InstitutionId = "123",
            CreatedByName = "cdcsdc",
            EndDate = "12/11/2025 13:50:02",
            InstitutionName = "hhh",
            Degree = "mnn",
            ContactPersonRole = "hh",
            Id = "2",
            Year = "12/11/2025 13:50:02",
            Address = "cddsc",
            University = "cdsc",
            UpdatedByName = "cdsc",
            ContactId = "90217",
            StartDate = "12/11/2025 13:50:02",
            PhoneNum = "12345678",
            DateLastAttended = "12/11/2025 13:50:02",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddContactEducationInfoForCasesToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactEducationTests>();
        List<ContactEducationJson> contactEducation = EducationJsons;

        var numberOfCaseContactEducationBeforeInsertion = realm.All<ContactEducation>().Count();
        await ContactEducation.SynchronizeAsync(realm, contactEducation, "10");

        var numberOfCaseContactEducationAfterInsertion = realm.All<ContactEducation>().Count();

        Assert.Equal(0, numberOfCaseContactEducationBeforeInsertion);
        Assert.Equal(2, numberOfCaseContactEducationAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesCaseContactEducationDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactEducationTests>();
        List<ContactEducationJson> contactEducation = [.. EducationJsons];
        string parentId = "10";

        await ContactEducation.SynchronizeAsync(realm, contactEducation, parentId);
        var numberOfCaseContactEducationBeforeDeletion = realm.All<ContactEducation>().Count();

        contactEducation.RemoveAll(item => item.Id == "1");

        await ContactEducation.SynchronizeAsync(realm, contactEducation, parentId);
        var numberOfCaseContactEducationAfterDeletion = realm.All<ContactEducation>().Count();

        Assert.Equal(2, numberOfCaseContactEducationBeforeDeletion);
        Assert.Equal(numberOfCaseContactEducationBeforeDeletion - 1, numberOfCaseContactEducationAfterDeletion);
    }
}
