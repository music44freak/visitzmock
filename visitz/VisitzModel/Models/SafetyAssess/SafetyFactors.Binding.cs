/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyFactors
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName == null)
            return;

        bool notBound = !propertyName.EndsWith(Binding);

        if (notBound)
        {
            RaisePropertyChanged($"{propertyName}{Binding}");

            if (IsQuestionPrompt(propertyName))
            {
                RaisePropertyChanged(nameof(AnyTrue));
                RaisePropertyChanged(nameof(AllAnswered));
            }
        }
    }

    private bool IsQuestionPrompt(string propertyName)
    {
        return GetType().GetProperty(propertyName)?.PropertyType == typeof(bool?);
    }

    public bool? PhysicalHarmBinding
    {
        get => IsValid ? PhysicalHarm : default;
        set => this.Commit(() => PhysicalHarm = value);
    }

    public bool SeriousInjuryAbuseBinding
    {
        get => IsValid && SeriousInjuryAbuse;
        set => this.Commit(() => SeriousInjuryAbuse = value);
    }

    public bool FearsMaltreatChildBinding
    {
        get => IsValid && FearsMaltreatChild;
        set => this.Commit(() => FearsMaltreatChild = value);
    }

    public bool ThreatAgainstChildBinding
    {
        get => IsValid && ThreatAgainstChild;
        set => this.Commit(() => ThreatAgainstChild = value);
    }

    public bool ExcessiveForceBinding
    {
        get => IsValid && ExcessiveForce;
        set => this.Commit(() => ExcessiveForce = value);
    }

    public bool SubsExposedInfantBinding
    {
        get => IsValid && SubsExposedInfant;
        set => this.Commit(() => SubsExposedInfant = value);
    }

    public string CmtClarificationBinding
    {
        get => IsValid ? CmtClarification : string.Empty;
        set => this.Commit(() => CmtClarification = value);
    }

    public bool? CurrentCircumstancesBinding
    {
        get => IsValid ? CurrentCircumstances : default;
        set => this.Commit(() => CurrentCircumstances = value);
    }

    public string CmtCircumstancesBinding
    {
        get => IsValid ? CmtCircumstances : string.Empty;
        set => this.Commit(() => CmtCircumstances = value);
    }

    public bool? SexAbuseBinding
    {
        get => IsValid ? SexAbuse : default;
        set => this.Commit(() => SexAbuse = value);
    }

    public string CmtAbuseBinding
    {
        get => IsValid ? CmtAbuse : string.Empty;
        set => this.Commit(() => CmtAbuse = value);
    }

    public bool? UnableToProtectBinding
    {
        get => IsValid ? UnableToProtect : default;
        set => this.Commit(() => UnableToProtect = value);
    }

    public string CmtProtectBinding
    {
        get => IsValid ? CmtProtect : string.Empty;
        set => this.Commit(() => CmtProtect = value);
    }

    public bool? InjuryExplanationBinding
    {
        get => IsValid ? InjuryExplanation : default;
        set => this.Commit(() => InjuryExplanation = value);
    }

    public string CmtExplanationBinding
    {
        get => IsValid ? CmtExplanation : string.Empty;
        set => this.Commit(() => CmtExplanation = value);
    }

    public bool? RefuseAccessBinding
    {
        get => IsValid ? RefuseAccess : default;
        set => this.Commit(() => RefuseAccess = value);
    }

    public string CmtAccessBinding
    {
        get => IsValid ? CmtAccess : string.Empty;
        set => this.Commit(() => CmtAccess = value);
    }

    public bool? ImmediateNeedsBinding
    {
        get => IsValid ? ImmediateNeeds : default;
        set => this.Commit(() => ImmediateNeeds = value);
    }

    public string CmtNeedsBinding
    {
        get => IsValid ? CmtNeeds : string.Empty;
        set => this.Commit(() => CmtNeeds = value);
    }

    public bool? PhysicalConditionBinding
    {
        get => IsValid ? PhysicalCondition : default;
        set => this.Commit(() => PhysicalCondition = value);
    }

    public string CmtConditionBinding
    {
        get => IsValid ? CmtCondition : string.Empty;
        set => this.Commit(() => CmtCondition = value);
    }

    public bool? CurrentAbuseBinding
    {
        get => IsValid ? CurrentAbuse : default;
        set => this.Commit(() => CurrentAbuse = value);
    }

    public string CmtCurrentBinding
    {
        get => IsValid ? CmtCurrent : string.Empty;
        set => this.Commit(() => CmtCurrent = value);
    }

    public bool? PartnerViolenceBinding
    {
        get => IsValid ? PartnerViolence : default;
        set => this.Commit(() => PartnerViolence = value);
    }

    public string CmtViolenceBinding
    {
        get => IsValid ? CmtViolence : string.Empty;
        set => this.Commit(() => CmtViolence = value);
    }

    public bool? PredominantlyNegativeBinding
    {
        get => IsValid ? PredominantlyNegative : default;
        set => this.Commit(() => PredominantlyNegative = value);
    }

    public string CmtNegativeBinding
    {
        get => IsValid ? CmtNegative : string.Empty;
        set => this.Commit(() => CmtNegative = value);
    }

    public bool? EmotionalStabilityBinding
    {
        get => IsValid ? EmotionalStability : default;
        set => this.Commit(() => EmotionalStability = value);
    }

    public string CmtEmotionalBinding
    {
        get => IsValid ? CmtEmotional : string.Empty;
        set => this.Commit(() => CmtEmotional = value);
    }

    public bool? ChildFearfulBinding
    {
        get => IsValid ? ChildFearful : default;
        set => this.Commit(() => ChildFearful = value);
    }

    public string CmtFearfulBinding
    {
        get => IsValid ? CmtFearful : string.Empty;
        set => this.Commit(() => CmtFearful = value);
    }

    public bool? OtherFactorsBinding
    {
        get => IsValid ? OtherFactors : default;
        set => this.Commit(() => OtherFactors = value);
    }

    public string CmtOtherFactorsBinding
    {
        get => IsValid ? CmtOtherFactors : string.Empty;
        set => this.Commit(() => CmtOtherFactors = value);
    }
}
