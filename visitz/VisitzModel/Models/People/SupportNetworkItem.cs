using Realms;
using VisitzApi.Models;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.People;

public partial class SupportNetworkItem
    : IRealmObject,
        IRowMetadata,
        IApiJson<SupportNetworkJson>,
        IParentRecord,
        IApiJson<SubmitSupportNetworkJson>
{
    [PrimaryKey]
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

    public string Active { get; set; }

    public string Address { get; set; }

    public string AgencyName { get; set; }

    public string ParentRecordId { get; set; }

    public string CellPhoneNumber { get; set; }

    public string Comments { get; set; }

    public string EmergencyContact { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string IcmSncCaseConFlag { get; set; }

    public string IcmSncSrConFlag { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Relationship { get; set; }

    public SupportNetworkItem() { }

    public SupportNetworkItem(SupportNetworkJson json, string parentId, EntityType type)
    {
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        ParentId = parentId;
        ParentType = type;
        Active = json.Active;
        Address = json.Address;
        AgencyName = json.Agency;
        CellPhoneNumber = json.Cell;
        Comments = json.Comments;
        EntityId = json.EntityId;
        EntityName = json.EntityName;
        Name = json.Name;
        PhoneNumber = json.Phone;
        Relationship = json.Relationship;
    }

    SupportNetworkJson IApiJson<SupportNetworkJson>.ToApiJson(string dateFormat = "s")
    {
        return new SupportNetworkJson()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            CreatedById = CreatedById,
            UpdatedBy = UpdatedBy,
            UpdatedById = UpdatedById,
            CreatedDate = CreatedDate.ToString(dateFormat),
            UpdatedDate = UpdatedDate.ToString(dateFormat),
            Active = Active,
            Address = Address,
            Agency = AgencyName,
            Cell = CellPhoneNumber,
            Comments = Comments,
            EntityId = EntityId,
            EntityName = EntityName,
            Name = Name,
            Phone = PhoneNumber,
            Relationship = Relationship,
        };
    }

    SubmitSupportNetworkJson IApiJson<SubmitSupportNetworkJson>.ToApiJson(string dateFormat = "s")
    {
        return new SubmitSupportNetworkJson()
        {
            Active = Active,
            Address = Address,
            AgencyName = AgencyName,
            Cell = CellPhoneNumber,
            Comments = Comments,
            Name = Name,
            Phone = PhoneNumber,
            Relationship = Relationship,
        };
    }

    public static IEnumerable<SupportNetworkItem> FromApiArray(
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type
    )
    {
        List<SupportNetworkItem> outList = [];

        foreach (var supportNetworkJson in items)
            outList.Add(new SupportNetworkItem(supportNetworkJson, parentId, type));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<SupportNetworkJson> items,
        string parentId,
        EntityType type
    )
    {
        var incomingSupportNetworkItems = FromApiArray(items, parentId, type);
        var incomingSupportNetworkItemIds = incomingSupportNetworkItems.Select(item => item.Id);
        var supportNetworks = realm
            .All<SupportNetworkItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type)
            .ToList();
        var supportNetworkIds = supportNetworks.Select(item => item.Id);

        var networkItemIdsToDelete = supportNetworkIds.Except(incomingSupportNetworkItemIds);
        var networkItemsToDelete = supportNetworks.Where(item => networkItemIdsToDelete.Contains(item.Id));

        await RealmExtensions.CommitAsync(
            realm,
            () =>
            {
                foreach (var item in supportNetworks)
                {
                    if (item != null && item.IsValid)
                        realm.Remove(item);
                }
                realm.Upsert(incomingSupportNetworkItems);
            }
        );
    }

    public static IQueryable<SupportNetworkItem> GetSupportNetworkByCaseId(Realm realm, string caseId)
    {
        return realm
            .All<SupportNetworkItem>()
            .Where(item => item.EntityId == caseId)
            .Filter($"TRUEPREDICATE SORT({nameof(Name)} ASC)");
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var networkItems = realm
            .All<SupportNetworkItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        realm.RemoveRange(networkItems);
    }
}
