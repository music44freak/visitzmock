using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.People;

#nullable enable
public partial class ContactLanguage : IRealmObject, IApiJson<ContactLanguageJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public string Type { get; set; } = string.Empty;
    public string SSAPrimaryField { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public string TranslatorReq { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public string ParentContactId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public string OtherLanguage { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string ICMType { get; set; } = string.Empty;

    public ContactLanguage() { }

    public ContactLanguage(ContactLanguageJson json, string parentContactId)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        Created = DateTimeOffset.Parse(json.Created);
        UpdatedBy = json.UpdatedBy;
        Type = json.Type;
        SSAPrimaryField = json.SSAPrimaryField;
        Updated = DateTimeOffset.Parse(json.Updated);
        TranslatorReq = json.TranslatorReq;
        Comments = json.Comments;
        UpdatedByName = json.UpdatedByName;
        ParentContactId = parentContactId;
        LanguageName = json.LanguageName;
        OtherLanguage = json.OtherLanguage;
        CreatedByName = json.CreatedByName;
        ICMType = json.ICMType;
    }

    public ContactLanguageJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            Created = Created.ToString(dateFormat) ?? string.Empty,
            UpdatedBy = UpdatedBy,
            Type = Type,
            SSAPrimaryField = SSAPrimaryField,
            Updated = Updated.ToString(dateFormat) ?? string.Empty,
            TranslatorReq = TranslatorReq,
            Comments = Comments,
            UpdatedByName = UpdatedByName,
            ContactId = ParentContactId,
            LanguageName = LanguageName,
            OtherLanguage = OtherLanguage,
            CreatedByName = CreatedByName,
            ICMType = ICMType,
        };
    }

    public static List<ContactLanguage> FromApiJsonArray(
        IEnumerable<ContactLanguageJson> jsonArray,
        string parentContactId
    )
    {
        List<ContactLanguage> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactLanguage(jsonItem, parentContactId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactLanguageJson> newContactLanguages,
        string parentContactId
    )
    {
        if (newContactLanguages == null)
            return;

        var incomingContactLanguages = FromApiJsonArray(newContactLanguages, parentContactId);
        var incomingContactLanguageIds = incomingContactLanguages.Select(item => item.Id);

        var allContactLanguages = realm.All<ContactLanguage>().Where(item => item.ParentContactId == parentContactId);
        var allContactLanguageIds = allContactLanguages.AsEnumerable().Select(item => item.Id);

        var contactLanguageIdsToDelete = allContactLanguageIds.Except(incomingContactLanguageIds);
        var contactLanguagesToDelete = allContactLanguages
            .ToList()
            .Where(item => contactLanguageIdsToDelete.Contains(item.Id));

        if (!contactLanguageIdsToDelete.Any() && !incomingContactLanguageIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactLanguagesToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }
                realm.Upsert(incomingContactLanguages);
            }
        );
    }

    public static void RemoveByParent(Realm realm, string parentContactId)
    {
        var contacts = realm.All<IcmContact>().Where(item => item.Id == parentContactId);

        if (contacts.Count() <= 1)
        {
            var contactLanguagesToBeDeleted = realm
                .All<ContactLanguage>()
                .Where(item => item.ParentContactId == parentContactId);

            realm.RemoveRange(contactLanguagesToBeDeleted);
        }
    }
}
