using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyInterventions : IRealmObject, IApiJson<SubmitSafetyInterventionsJson>
{
    public bool DirectIntervention { get; set; }

    public bool UseOfIndividuals { get; set; }

    public bool UseCommAgencies { get; set; }

    public bool ProtectVictim { get; set; }

    public bool LeaveHome { get; set; }

    public bool NonOffendingParent { get; set; }

    public bool LegalIntPlanned { get; set; }

    public bool OtherSafetyInterventions { get; set; }

    public string CmtSafetyInterventions { get; set; } = string.Empty;

    public bool ChildOutsideHome { get; set; }

    public bool ChildRemoved { get; set; }

    public static SafetyInterventions FromApiJson(GetSafetyAsessmentJson json)
    {
        return new SafetyInterventions()
        {
            DirectIntervention = json.SafetyIntervention01.ParseWordTruthiness(),
            UseOfIndividuals = json.SafetyIntervention02.ParseWordTruthiness(),
            UseCommAgencies = json.SafetyIntervention03.ParseWordTruthiness(),
            ProtectVictim = json.SafetyIntervention04.ParseWordTruthiness(),
            LeaveHome = json.SafetyIntervention05.ParseWordTruthiness(),
            NonOffendingParent = json.SafetyIntervention06.ParseWordTruthiness(),
            LegalIntPlanned = json.SafetyIntervention07.ParseWordTruthiness(),
            OtherSafetyInterventions = json.SafetyIntervention08.ParseWordTruthiness(),
            CmtSafetyInterventions = json.SafetyIntervention08Other,
            ChildOutsideHome = json.SafetyIntervention09.ParseWordTruthiness(),
            ChildRemoved = json.SafetyIntervention10.ParseWordTruthiness(),
        };
    }

    public SubmitSafetyInterventionsJson ToApiJson(string _ = "s")
    {
        return new SubmitSafetyInterventionsJson()
        {
            DirectIntervention = DirectIntervention.AsTruthyChar(),
            UseOfIndividuals = UseOfIndividuals.AsTruthyChar(),
            UseCommAgencies = UseCommAgencies.AsTruthyChar(),
            ProtectVictim = ProtectVictim.AsTruthyChar(),
            LeaveHome = LeaveHome.AsTruthyChar(),
            NonOffendingParent = NonOffendingParent.AsTruthyChar(),
            LegalIntPlanned = LegalIntPlanned.AsTruthyChar(),
            OtherSafetyInterventions = OtherSafetyInterventions.AsTruthyChar(),
            CmtSafetyInterventions = OtherSafetyInterventions ? CmtSafetyInterventions : "",
            ChildOutsideHome = ChildOutsideHome.AsTruthyChar(),
            ChildRemoved = ChildRemoved.AsTruthyChar(),
        };
    }
}
