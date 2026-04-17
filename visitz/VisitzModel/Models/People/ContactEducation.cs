using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

#nullable enable

namespace VisitzModel.Models.People;

public partial class ContactEducation : IRealmObject, IApiJson<ContactEducationJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LearningAssistant { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string SchoolName { get; set; } = string.Empty;
    public string IndividualEducationPlan { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string InstitutionId { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTimeOffset? EndDate { get; set; } = DateTimeOffset.UtcNow;
    public string InstitutionName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string ContactPersonRole { get; set; } = string.Empty;
    public DateTimeOffset? Year { get; set; } = DateTimeOffset.UtcNow;
    public string Address { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.UtcNow;
    public string PhoneNum { get; set; } = string.Empty;
    public DateTimeOffset? DateLastAttended { get; set; } = DateTimeOffset.UtcNow;
    public string ParentContactId { get; set; } = string.Empty;

    public ContactEducation() { }

    public ContactEducation(ContactEducationJson json, string parentContactId)
    {
        Id = json.Id;
        LearningAssistant = json.LearningAssistant;
        SchoolName = json.SchoolName;
        IndividualEducationPlan = json.IndividualEducationPlan;
        Comments = json.Comments;
        InstitutionId = json.InstitutionId;
        InstitutionName = json.InstitutionName;
        CreatedByName = json.CreatedByName;
        EndDate = Timestamp.ParseDateTimeOffsetNullable(json.EndDate);
        Degree = json.Degree;
        ContactPersonRole = json.ContactPersonRole;
        Year = Timestamp.ParseDateTimeOffsetNullable(json.Year);
        Address = json.Address;
        University = json.University;
        UpdatedByName = json.UpdatedByName;
        PhoneNum = json.PhoneNum;
        StartDate = Timestamp.ParseDateTimeOffsetNullable(json.StartDate);
        DateLastAttended = Timestamp.ParseDateTimeOffsetNullable(json.DateLastAttended);
        ParentContactId = parentContactId;
    }

    public ContactEducationJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            LearningAssistant = LearningAssistant,
            Address = Address,
            Comments = Comments,
            ContactId = ParentContactId,
            ContactPerson = ContactPerson,
            ContactPersonRole = ContactPersonRole,
            CreatedByName = CreatedByName,
            DateLastAttended = DateLastAttended?.ToString(dateFormat) ?? string.Empty,
            Degree = Degree,
            EndDate = EndDate?.ToString(dateFormat) ?? string.Empty,
            IndividualEducationPlan = IndividualEducationPlan,
            InstitutionId = InstitutionId,
            InstitutionName = InstitutionName,
            PhoneNum = PhoneNum,
            SchoolName = SchoolName,
            StartDate = DateLastAttended?.ToString(dateFormat) ?? string.Empty,
            University = University,
            UpdatedByName = UpdatedByName,
            Year = Year?.ToString(dateFormat) ?? string.Empty,
        };
    }

    public static List<ContactEducation> FromApiJsonArray(
        IEnumerable<ContactEducationJson> jsonArray,
        string parentContactId
    )
    {
        List<ContactEducation> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactEducation(jsonItem, parentContactId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactEducationJson> contactEducation,
        string parentContactId
    )
    {
        if (contactEducation == null)
            return;

        var incomingContactEducation = FromApiJsonArray(contactEducation, parentContactId);
        var incomingContactEducationIds = incomingContactEducation.Select(item => item.Id);

        var allContactEducation = realm.All<ContactEducation>().Where(item => item.ParentContactId == parentContactId);
        var allContactEducationIds = allContactEducation.AsEnumerable().Select(item => item.Id);

        var contactEducationIdsToDelete = allContactEducationIds.Except(incomingContactEducationIds);
        var contactEducationToDelete = allContactEducation
            .ToList()
            .Where(item => contactEducationIdsToDelete.Contains(item.Id));

        if (!contactEducationToDelete.Any() && !incomingContactEducationIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactEducationToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingContactEducation);
            }
        );
    }

    public static void RemoveByParent(Realm realm, string parentContactId)
    {
        var contacts = realm.All<IcmContact>().Where(item => item.Id == parentContactId);

        if (contacts.Count() <= 1)
        {
            var contactEducationToBeDeleted = realm
                .All<ContactEducation>()
                .Where(item => item.ParentContactId == parentContactId);

            realm.RemoveRange(contactEducationToBeDeleted);
        }
    }
}
