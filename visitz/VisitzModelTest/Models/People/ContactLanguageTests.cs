using VisitzApi.Models.People;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class ContactLanguageTests
{
    private static readonly List<ContactLanguageJson> GetContactlanguageInformation =
    [
        new()
        {
            Created = "12/10/2026 13:50:02",
            CreatedBy = "ram",
            CreatedByName = "abc",
            Id = "11",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            Type = "Test",
            SSAPrimaryField = "dghweg",
            TranslatorReq = "TranslatorReq",
            Comments = "Comments",
            ContactId = "456",
            LanguageName = "english",
            OtherLanguage = "hindi",
            ICMType = "123",
        },
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "sala",
            CreatedByName = "abc",
            Id = "12",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test1",
            Type = "Test",
            SSAPrimaryField = "dghweg",
            TranslatorReq = "TranslatorRequ",
            Comments = "Commentss",
            ContactId = "123",
            LanguageName = "english",
            OtherLanguage = "hindi",
            ICMType = "456",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddcontactlanguageToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLanguageTests>();
        List<ContactLanguageJson> contactLanguages = GetContactlanguageInformation;
        var numberOfContactLanguagesBeforeInsertion = realm.All<ContactLanguage>().Count();
        await ContactLanguage.SynchronizeAsync(realm, contactLanguages, "123");
        var numberOfContactLanguagesAfterInsertion = realm.All<ContactLanguage>().Count();
        Assert.Equal(0, numberOfContactLanguagesBeforeInsertion);
        Assert.Equal(2, numberOfContactLanguagesAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletescontactlanguageDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactLanguageTests>();
        List<ContactLanguageJson> contactLanguages = [.. GetContactlanguageInformation];
        await ContactLanguage.SynchronizeAsync(realm, contactLanguages, "123");
        var numberOfContactLanguagesBeforeDeletion = realm.All<ContactLanguage>().Count();
        contactLanguages.RemoveAll(item => item.Id == "11");
        await ContactLanguage.SynchronizeAsync(realm, contactLanguages, "123");
        var numberOfContactLanguagesAfterDeletion = realm.All<ContactLanguage>().Count();
        var inc = realm.All<ContactLanguage>().ToList();
        Assert.Equal(2, numberOfContactLanguagesBeforeDeletion);
        Assert.Equal(numberOfContactLanguagesBeforeDeletion - 1, numberOfContactLanguagesAfterDeletion);
    }
}
