using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;

#nullable enable

namespace VisitzModel.Models.People;

public partial class ContactLegalAuditTrail : IRealmObject, IApiJson<ContactLegalAuditTrailJson>
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    public string OperationPerformed { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
    public string UpdatedByName { get; set; } = string.Empty;
    public string Updatedby { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LegalAuthorityCode { get; set; } = string.Empty;
    public string EmployeeLogin { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string CreatedbyName { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    private int ParentTypeInt { get; set; } = (int)EntityType.Unknown;
    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public ContactLegalAuditTrail() { }

    public ContactLegalAuditTrail(ContactLegalAuditTrailJson json, EntityType type, string parentId)
    {
        Id = json.ID;
        OperationPerformed = json.OperationPerformed;
        Type = json.Type;
        CreatedBy = json.CreatedBy;
        Updated = DateTimeOffset.Parse(json.Updated);
        Updatedby = json.Updatedby;
        Created = DateTimeOffset.Parse(json.Created);
        UpdatedByName = json.UpdatedByName;
        LegalAuthorityCode = json.LegalAuthorityCode;
        Type = json.Type;
        EmployeeLogin = json.EmployeeLogin;
        EntityId = json.EntityId;
        CreatedbyName = json.CreatedbyName;
        ParentType = type;
        ParentId = parentId;
    }

    public ContactLegalAuditTrailJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            ID = Id,
            Created = Created.ToString(dateFormat) ?? string.Empty,
            CreatedBy = CreatedBy,
            CreatedbyName = CreatedbyName,
            Updated = Updated.ToString(dateFormat) ?? string.Empty,
            Updatedby = Updatedby,
            UpdatedByName = UpdatedByName,
            OperationPerformed = OperationPerformed,
            Type = Type,
            LegalAuthorityCode = LegalAuthorityCode,
            EmployeeLogin = EmployeeLogin,
            EntityId = EntityId,
        };
    }

    public static List<ContactLegalAuditTrail> FromApiJsonArray(
        IEnumerable<ContactLegalAuditTrailJson> jsonArray,
        EntityType type,
        string parentId
    )
    {
        List<ContactLegalAuditTrail> outList = [];

        if (jsonArray != null)
            foreach (var jsonItem in jsonArray)
                outList.Add(new ContactLegalAuditTrail(jsonItem, type, parentId));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactLegalAuditTrailJson> contactLegalAuditTrail,
        string parentId,
        EntityType type
    )
    {
        if (contactLegalAuditTrail == null)
            return;

        var incomingContactLegalAuditTrail = FromApiJsonArray(contactLegalAuditTrail, type, parentId);
        var incomingContactLegalAuditTrailIds = incomingContactLegalAuditTrail.Select(item => item.Id);

        var allContactLegalAuditTrail = realm
            .All<ContactLegalAuditTrail>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);
        var allContactLegalAuditTrailIds = allContactLegalAuditTrail.AsEnumerable().Select(item => item.Id);

        var contactLegalAuditTrailIdsToDelete = allContactLegalAuditTrailIds.Except(incomingContactLegalAuditTrailIds);
        var contactLegalAuditTrailToDelete = allContactLegalAuditTrail
            .ToList()
            .Where(item => contactLegalAuditTrailIdsToDelete.Contains(item.Id));

        if (!contactLegalAuditTrailToDelete.Any() && !incomingContactLegalAuditTrailIds.Any())
            return;

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in contactLegalAuditTrailToDelete)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }

                realm.Upsert(incomingContactLegalAuditTrail);
            }
        );
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var contactMedicalBehavioral = realm
            .All<ContactLegalAuditTrail>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type)
            .ToList();

        foreach (var item in contactMedicalBehavioral)
        {
            realm.Remove(item);
        }
    }
}
