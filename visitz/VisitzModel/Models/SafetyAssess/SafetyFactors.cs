using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyFactors : IRealmObject, IApiJson<SubmitSafetyFactorsJson>
{
    public bool? PhysicalHarm { get; set; }

    public bool SeriousInjuryAbuse { get; set; }

    public bool FearsMaltreatChild { get; set; }

    public bool ThreatAgainstChild { get; set; }

    public bool ExcessiveForce { get; set; }

    public bool SubsExposedInfant { get; set; }

    public string CmtClarification { get; set; } = string.Empty;

    public bool? CurrentCircumstances { get; set; }

    public string CmtCircumstances { get; set; } = string.Empty;

    public bool? SexAbuse { get; set; }

    public string CmtAbuse { get; set; } = string.Empty;

    public bool? UnableToProtect { get; set; }

    public string CmtProtect { get; set; } = string.Empty;

    public bool? InjuryExplanation { get; set; }

    public string CmtExplanation { get; set; } = string.Empty;

    public bool? RefuseAccess { get; set; }

    public string CmtAccess { get; set; } = string.Empty;

    public bool? ImmediateNeeds { get; set; }

    public string CmtNeeds { get; set; } = string.Empty;

    public bool? PhysicalCondition { get; set; }

    public string CmtCondition { get; set; } = string.Empty;

    public bool? CurrentAbuse { get; set; }

    public string CmtCurrent { get; set; } = string.Empty;

    public bool? PartnerViolence { get; set; }

    public string CmtViolence { get; set; } = string.Empty;

    public bool? PredominantlyNegative { get; set; }

    public string CmtNegative { get; set; } = string.Empty;

    public bool? EmotionalStability { get; set; }

    public string CmtEmotional { get; set; } = string.Empty;

    public bool? ChildFearful { get; set; }

    public string CmtFearful { get; set; } = string.Empty;

    public bool? OtherFactors { get; set; }

    public string CmtOtherFactors { get; set; } = string.Empty;

    /// <summary>
    /// Unused
    /// </summary>
    public bool? CurretAbuse { get; set; }

    public bool AnyTrue =>
        (PhysicalHarm ?? false)
        || (CurrentCircumstances ?? false)
        || (SexAbuse ?? false)
        || (UnableToProtect ?? false)
        || (InjuryExplanation ?? false)
        || (RefuseAccess ?? false)
        || (ImmediateNeeds ?? false)
        || (PhysicalCondition ?? false)
        || (CurrentAbuse ?? false)
        || (PartnerViolence ?? false)
        || (PredominantlyNegative ?? false)
        || (EmotionalStability ?? false)
        || (ChildFearful ?? false)
        || (OtherFactors ?? false);

    public bool AllAnswered =>
        PhysicalHarm != null
        && CurrentCircumstances != null
        && SexAbuse != null
        && UnableToProtect != null
        && InjuryExplanation != null
        && RefuseAccess != null
        && ImmediateNeeds != null
        && PhysicalCondition != null
        && CurrentAbuse != null
        && PartnerViolence != null
        && PredominantlyNegative != null
        && EmotionalStability != null
        && ChildFearful != null
        && OtherFactors != null;

    public static SafetyFactors FromApiJson(GetSafetyAsessmentJson json)
    {
        return new SafetyFactors()
        {
            PhysicalHarm = json.SafetyFactor01.ParseWordTruthiness(),
            SeriousInjuryAbuse = json.SafetyFactor01A.ParseWordTruthiness(),
            FearsMaltreatChild = json.SafetyFactor01B.ParseWordTruthiness(),
            ThreatAgainstChild = json.SafetyFactor01C.ParseWordTruthiness(),
            ExcessiveForce = json.SafetyFactor01D.ParseWordTruthiness(),
            SubsExposedInfant = json.SafetyFactor01E.ParseWordTruthiness(),
            CmtClarification = json.SafetyFactor01Comment,
            CurrentCircumstances = json.SafetyFactor02.ParseWordTruthiness(),
            CmtCircumstances = json.SafetyFactor02Comment,
            SexAbuse = json.SafetyFactor03.ParseWordTruthiness(),
            CmtAbuse = json.SafetyFactor03Comment,
            UnableToProtect = json.SafetyFactor04.ParseWordTruthiness(),
            CmtProtect = json.SafetyFactor04Comment,
            InjuryExplanation = json.SafetyFactor05.ParseWordTruthiness(),
            CmtExplanation = json.SafetyFactor05Comment,
            RefuseAccess = json.SafetyFactor06.ParseWordTruthiness(),
            CmtAccess = json.SafetyFactor06Comment,
            ImmediateNeeds = json.SafetyFactor07.ParseWordTruthiness(),
            CmtNeeds = json.SafetyFactor07Comment,
            PhysicalCondition = json.SafetyFactor08.ParseWordTruthiness(),
            CmtCondition = json.SafetyFactor08Comment,
            CurrentAbuse = json.SafetyFactor09.ParseWordTruthiness(),
            CmtCurrent = json.SafetyFactor09Comment,
            PartnerViolence = json.SafetyFactor10.ParseWordTruthiness(),
            CmtViolence = json.SafetyFactor10Comment,
            PredominantlyNegative = json.SafetyFactor11.ParseWordTruthiness(),
            CmtNegative = json.SafetyFactor11Comment,
            EmotionalStability = json.SafetyFactor12.ParseWordTruthiness(),
            CmtEmotional = json.SafetyFactor12Comment,
            ChildFearful = json.SafetyFactor13.ParseWordTruthiness(),
            CmtFearful = json.SafetyFactor13Comment,
            OtherFactors = json.SafetyFactor14.ParseWordTruthiness(),
            CmtOtherFactors = json.SafetyFactor14Comment,
            CurretAbuse = null,
        };
    }

    public SubmitSafetyFactorsJson ToApiJson(string _ = "s")
    {
        return new SubmitSafetyFactorsJson()
        {
            PhysicalHarm = PhysicalHarm?.AsTruthyWord() ?? string.Empty,
            SeriousInjuryAbuse = SeriousInjuryAbuse.AsTruthyChar(),
            FearsMaltreatChild = FearsMaltreatChild.AsTruthyChar(),
            ThreatAgainstChild = ThreatAgainstChild.AsTruthyChar(),
            ExcessiveForce = ExcessiveForce.AsTruthyChar(),
            SubsExposedInfant = SubsExposedInfant.AsTruthyChar(),
            CmtClarification = CmtClarification,
            CurrentCircumstances = CurrentCircumstances?.AsTruthyWord() ?? string.Empty,
            CmtCircumstances = CmtCircumstances,
            SexAbuse = SexAbuse?.AsTruthyWord() ?? string.Empty,
            CmtAbuse = CmtAbuse,
            UnableToProtect = UnableToProtect?.AsTruthyWord() ?? string.Empty,
            CmtProtect = CmtProtect,
            InjuryExplanation = InjuryExplanation?.AsTruthyWord() ?? string.Empty,
            CmtExplanation = CmtExplanation,
            RefuseAccess = RefuseAccess?.AsTruthyWord() ?? string.Empty,
            CmtAccess = CmtAccess,
            ImmediateNeeds = ImmediateNeeds?.AsTruthyWord() ?? string.Empty,
            CmtNeeds = CmtNeeds,
            PhysicalCondition = PhysicalCondition?.AsTruthyWord() ?? string.Empty,
            CmtCondition = CmtCondition,
            CurrentAbuse = CurrentAbuse?.AsTruthyWord() ?? string.Empty,
            CmtCurrent = CmtCurrent,
            PartnerViolence = PartnerViolence?.AsTruthyWord() ?? string.Empty,
            CmtViolence = CmtViolence,
            PredominantlyNegative = PredominantlyNegative?.AsTruthyWord() ?? string.Empty,
            CmtNegative = CmtNegative,
            EmotionalStability = EmotionalStability?.AsTruthyWord() ?? string.Empty,
            CmtEmotional = CmtEmotional,
            ChildFearful = ChildFearful?.AsTruthyWord() ?? string.Empty,
            CmtFearful = CmtFearful,
            OtherFactors = OtherFactors?.AsTruthyWord() ?? string.Empty,
            CmtOtherFactors = CmtOtherFactors,
            CurretAbuse = CurretAbuse?.AsTruthyWord() ?? string.Empty,
        };
    }
}
