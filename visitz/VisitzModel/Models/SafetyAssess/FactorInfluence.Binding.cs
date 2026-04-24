/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class FactorInfluence
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName != null && !propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public bool AgeUptoFiveBinding
    {
        get => IsValid && AgeUptoFive;
        set => this.Commit(() => AgeUptoFive = value);
    }

    public bool MedicalMentalDisorderBinding
    {
        get => IsValid && MedicalMentalDisorder;
        set => this.Commit(() => MedicalMentalDisorder = value);
    }

    public bool NotReadilyAccessibleBinding
    {
        get => IsValid && NotReadilyAccessible;
        set => this.Commit(() => NotReadilyAccessible = value);
    }

    public bool DiminishedMentalBinding
    {
        get => IsValid && DiminishedMental;
        set => this.Commit(() => DiminishedMental = value);
    }

    public bool DiminishedPhysicalBinding
    {
        get => IsValid && DiminishedPhysical;
        set => this.Commit(() => DiminishedPhysical = value);
    }
}
