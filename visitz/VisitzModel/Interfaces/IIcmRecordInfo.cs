using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Interfaces;

public interface IIcmRecordInfo
{
    public string RowId { get; set; }

    public EntityType EntityType { get; set; }
}
