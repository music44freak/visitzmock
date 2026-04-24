using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class ProtectiveCapacity : IRealmObject, IApiJson<SubmitProtectiveCapacityJson>
{
    public bool ChildCognitive { get; set; }

    public bool ParentCognitive { get; set; }

    public bool ParentWillingness { get; set; }

    public bool ParentResources { get; set; }

    public bool ParentSupportive { get; set; }

    public bool ParentProtect { get; set; }

    public bool ParentAccept { get; set; }

    public bool ParentRelationship { get; set; }

    public bool ParentAware { get; set; }

    public bool ParentProbSolving { get; set; }

    public bool NoProCapPresent { get; set; }

    public bool CapacitiesOther { get; set; }

    public string CmtProtectiveCapacity01 { get; set; } = string.Empty;

    public string CmtProtectiveCapacity02 { get; set; } = string.Empty;

    public static ProtectiveCapacity FromApiJson(GetSafetyAsessmentJson json)
    {
        return new ProtectiveCapacity()
        {
            ChildCognitive = json.ProtectiveCapacity01.ParseWordTruthiness(),
            ParentCognitive = json.ProtectiveCapacity02.ParseWordTruthiness(),
            ParentWillingness = json.ProtectiveCapacity03.ParseWordTruthiness(),
            ParentResources = json.ProtectiveCapacity04.ParseWordTruthiness(),
            ParentSupportive = json.ProtectiveCapacity05.ParseWordTruthiness(),
            ParentProtect = json.ProtectiveCapacity06.ParseWordTruthiness(),
            ParentAccept = json.ProtectiveCapacity07.ParseWordTruthiness(),
            ParentRelationship = json.ProtectiveCapacity08.ParseWordTruthiness(),
            ParentAware = json.ProtectiveCapacity09.ParseWordTruthiness(),
            ParentProbSolving = json.ProtectiveCapacity10.ParseWordTruthiness(),
            NoProCapPresent = json.ProtectiveCapacity11.ParseWordTruthiness(),
            CapacitiesOther = json.ProtectiveCapacity12.ParseWordTruthiness(),
            CmtProtectiveCapacity01 = json.ProtectiveCapacity12Other,
            CmtProtectiveCapacity02 = json.ProtectiveCapacityObservations,
        };
    }

    public SubmitProtectiveCapacityJson ToApiJson(string _ = "s")
    {
        return new SubmitProtectiveCapacityJson()
        {
            ChildCognitive = ChildCognitive.AsTruthyChar(),
            ParentCognitive = ParentCognitive.AsTruthyChar(),
            ParentWillingness = ParentWillingness.AsTruthyChar(),
            ParentResources = ParentResources.AsTruthyChar(),
            ParentSupportive = ParentSupportive.AsTruthyChar(),
            ParentProtect = ParentProtect.AsTruthyChar(),
            ParentAccept = ParentAccept.AsTruthyChar(),
            ParentRelationship = ParentRelationship.AsTruthyChar(),
            ParentAware = ParentAware.AsTruthyChar(),
            ParentProbSolving = ParentProbSolving.AsTruthyChar(),
            NoProCapPresent = NoProCapPresent.AsTruthyChar(),
            CapacitiesOther = CapacitiesOther.AsTruthyChar(),
            CmtProtectiveCapacity01 = CapacitiesOther ? CmtProtectiveCapacity01 : "",
            CmtProtectiveCapacity02 = CmtProtectiveCapacity02,
        };
    }

    /// <summary>
    /// Used for form business logic: "Protective Capacities section - Checking 'No protective capacities present'
    /// unchecks all other Protective Capacities checkboxes. Checking any other checkbox unchecks "No protective
    /// capacities present" checkbox".
    /// </summary>
    private void ClearNoProCapPresent()
    {
        NoProCapPresentBinding = false;
    }

    /// <summary>
    /// Used for form business logic: "Protective Capacities section - Checking 'No protective capacities present'
    /// unchecks all other Protective Capacities checkboxes. Checking any other checkbox unchecks "No protective
    /// capacities present" checkbox".
    /// </summary>
    /// <param name="newVal">Assigned directly to NoProCapPresent</param>
    private void SetNoProCapPresent(bool newVal)
    {
        NoProCapPresent = newVal;

        if (NoProCapPresent)
        {
            ParentCognitive =
                ParentWillingness =
                ParentResources =
                ParentSupportive =
                ParentProtect =
                ParentAccept =
                ParentRelationship =
                ParentAware =
                ParentProbSolving =
                CapacitiesOther =
                    false;
        }
    }
}
