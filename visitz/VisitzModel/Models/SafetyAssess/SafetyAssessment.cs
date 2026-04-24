using System.Globalization;
using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyAssessment : IRealmObject, IRowMetadata, IApiJson<SubmitSafetyAssessmentJson>
{
    static readonly string IdString = "{0}|{1}|{2}|LOCALONLY";

    public static readonly string DateFormat = "MM/dd/yyyy";
    public static readonly int CommentsMaxLength = 1000;
    public static readonly string DefaultOperation = "Insert";

    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedById { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string IncidentNumber { get; set; } = string.Empty;

    public string WorkerId { get; set; } = string.Empty;

    public string FamilyName { get; set; } = string.Empty;

    public DateTimeOffset? DateOfAssessment { get; set; }

    public string Operation { get; set; } = string.Empty;

    public FactorInfluence? FactorInfluence { get; set; } = new();

    public SafetyFactors? SafetyFactors { get; set; } = new();

    public ProtectiveCapacity? ProtectiveCapacity { get; set; } = new();

    public SafetyInterventions? SafetyInterventions { get; set; } = new();

    public SafetyDecisions? SafetyDecisions { get; set; } = new();

    public IList<string> ChildsInOutCare { get; } = null!;

    public string ApprovedBy { get; set; } = string.Empty;

    public string ApprovedDate { get; set; } = string.Empty;

    public string ApprovedToFinalize { get; set; } = string.Empty;

    public DateTimeOffset? ApprovedToFinalizeDate { get; set; }

    public DateTimeOffset? FinalizedDate { get; set; }

    public string ApprovedToFinalizeDS { get; set; } = string.Empty;

    public string DataStewardRole { get; set; } = string.Empty;

    public string SocialWorkerFirstName { get; set; } = string.Empty;

    public string SocialWorkerId { get; set; } = string.Empty;

    public string SocialWorkerLastName { get; set; } = string.Empty;

    public string TeamLeaderFirstName { get; set; } = string.Empty;

    public string TeamLeaderId { get; set; } = string.Empty;

    public string TeamLeaderLastName { get; set; } = string.Empty;

    public string TeamLeaderLoginName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public static SafetyAssessment FromApiJson(string fileNumber, GetSafetyAsessmentJson json)
    {
        var safetyAssessment = new SafetyAssessment()
        {
            Id = GetOrMakeId(fileNumber, json),
            CreatedBy = json.CreatedBy,
            CreatedById = json.CreatedById,
            CreatedDate = DateTimeOffset.Parse(json.CreatedDate),
            UpdatedBy = json.UpdatedBy,
            UpdatedById = json.UpdatedById,
            UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate),
            IncidentNumber = fileNumber,
            WorkerId = json.CreatedBy,
            FamilyName = json.FamilyName,
            DateOfAssessment = Timestamp.ParseDateTimeOffsetNullable(json.DateOfAssessment),
            Operation = "",
            ApprovedBy = json.ApprovedBy,
            ApprovedDate = json.ApprovedDate,
            ApprovedToFinalize = json.ApprovedToFinalize,
            ApprovedToFinalizeDate = Timestamp.ParseDateTimeOffsetNullable(json.ApprovedToFinalizeDate),
            FinalizedDate = Timestamp.ParseDateTimeOffsetNullable(json.FinalizedDate),
            ApprovedToFinalizeDS = json.ApprovedToFinalizeDS,
            DataStewardRole = json.DataStewardRole,
            SocialWorkerFirstName = json.SocialWorkerFirstName,
            SocialWorkerId = json.SocialWorkerId,
            SocialWorkerLastName = json.SocialWorkerLastName,
            TeamLeaderFirstName = json.TeamLeaderFirstName,
            TeamLeaderId = json.TeamLeaderId,
            TeamLeaderLastName = json.TeamLeaderLastName,
            TeamLeaderLoginName = json.TeamLeaderLoginName,
            Type = json.Type,
            FactorInfluence = FactorInfluence.FromApiJson(json),
            SafetyFactors = SafetyFactors.FromApiJson(json),
            ProtectiveCapacity = ProtectiveCapacity.FromApiJson(json),
            SafetyInterventions = SafetyInterventions.FromApiJson(json),
            SafetyDecisions = SafetyDecisions.FromApiJson(json),
        };

        foreach (var contact in json.ContactsInOutCare)
            safetyAssessment.ChildsInOutCare.Add(contact.Id);

        return safetyAssessment;
    }

    public static SafetyAssessment Make(string fileNumber, string workerId, string familyName)
    {
        return new SafetyAssessment()
        {
            Id = GetOrMakeId(fileNumber, workerId),
            IncidentNumber = fileNumber,
            WorkerId = workerId,
            FamilyName = familyName,
            Operation = DefaultOperation,
            DateOfAssessment = DateTimeOffset.Now,
            FactorInfluence = new FactorInfluence(),
            SafetyFactors = new SafetyFactors(),
            ProtectiveCapacity = new ProtectiveCapacity(),
            SafetyInterventions = new SafetyInterventions(),
            SafetyDecisions = new SafetyDecisions(),
        };
    }

    static string GetOrMakeId(string fileNumber, GetSafetyAsessmentJson json)
    {
        return GetOrMakeId(fileNumber, json.CreatedDate, json.CreatedBy, json.Id);
    }

    static string GetOrMakeId(string fileNumber, string createdBy)
    {
        return GetOrMakeId(fileNumber, DateTimeOffset.Now.ToString(), createdBy);
    }

    static string GetOrMakeId(string fileNumber, string createdDate, string createdBy, string? id = null)
    {
        return string.IsNullOrWhiteSpace(id) ? string.Format(IdString, fileNumber, createdDate, createdBy) : id;
    }

    public static IEnumerable<SafetyAssessment> FromApiJson(string incidentId, IEnumerable<GetSafetyAsessmentJson> json)
    {
        List<SafetyAssessment> assessments = [];

        foreach (var assessment in json)
            assessments.Add(FromApiJson(incidentId, assessment));

        return assessments;
    }

    public SubmitSafetyAssessmentJson ToApiJson(string dateFormat = "s")
    {
        ArgumentNullException.ThrowIfNull(FactorInfluence);
        ArgumentNullException.ThrowIfNull(SafetyFactors);
        ArgumentNullException.ThrowIfNull(ProtectiveCapacity);
        ArgumentNullException.ThrowIfNull(SafetyInterventions);
        ArgumentNullException.ThrowIfNull(SafetyDecisions);

        var safetyAssessmentEntity = new SubmitSafetyAssessmentJson()
        {
            Payload =
            [
                new()
                {
                    IncidentNumber = IncidentNumber,
                    FamilyName = FamilyName,
                    DateOfAssessment =
                        DateOfAssessment?.ToString(DateFormat, CultureInfo.InvariantCulture) ?? string.Empty,
                },
            ],
            FactorInfluence = [FactorInfluence.ToApiJson(dateFormat)],
            SafetyFactors = [SafetyFactors.ToApiJson(dateFormat)],
            ProtectiveCapacity = [ProtectiveCapacity.ToApiJson(dateFormat)],
            SafetyInterventions = [SafetyInterventions.ToApiJson(dateFormat)],
            SafetyDecisions = [SafetyDecisions.ToApiJson(dateFormat)],
        };

        foreach (var childId in ChildsInOutCare)
            safetyAssessmentEntity.AddChildContactId(childId);

        return safetyAssessmentEntity;
    }

    public static SafetyAssessment? FindByIncidentNumber(Realm realm, string incidentNumber)
    {
        bool query(SafetyAssessment sa) => sa.IncidentNumber.Equals(incidentNumber);

        // Running the FirstOrDefault() query without checking Any() first sometimes leads to an uncatchable
        // ArgumentOutOfRangeException. I wasn't able to track down the root cause of it, so this workaround
        // will have to do for now.
        //
        // https://github.com/realm/realm-dotnet/issues/3090#issuecomment-1313661344
        // https://github.com/realm/realm-dotnet/issues/3092
        // https://github.com/realm/realm-dotnet/issues/3333

        return realm.All<SafetyAssessment>().Any(query)
            ? realm.All<SafetyAssessment>().Where(query).FirstOrDefault()
            : null;
    }

    public static IQueryable<SafetyAssessment> GetAllByFileNumber(Realm realm, string fileNumber)
    {
        return realm.All<SafetyAssessment>().Where(a => a.IncidentNumber == fileNumber);
    }

    public static async Task Delete(Realm realm, SafetyAssessment safetyAssessment)
    {
        await realm.WriteAsync(() => realm.Remove(safetyAssessment));
    }

    public static async Task SynchronizeAsync(Realm realm, string fileNumber, IEnumerable<SafetyAssessment> assessments)
    {
        var newIds = assessments.Select(a => a.Id);

        var idsToRemove = GetAllByFileNumber(realm, fileNumber).AsEnumerable().Select(a => a.Id).Except(newIds);

        await realm.CommitAsync(() =>
        {
            realm.DeleteByIds<SafetyAssessment>(idsToRemove);
            realm.Upsert(assessments);
        });
    }
}
