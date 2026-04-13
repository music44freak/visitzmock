using System;
using System.Collections.Generic;
using System.Text;

namespace VisitzApi.Models.People;

#nullable enable
public class ContactLegalAuditTrailJson
{
    public string Created { get; set; } = string.Empty;
    public string OperationPerformed { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Updated { get; set; } = string.Empty;
    public string ID { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
    public string Updatedby { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string LegalAuthorityCode { get; set; } = string.Empty;
    public string EmployeeLogin { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string CreatedbyName { get; set; } = string.Empty;
}
