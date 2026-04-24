/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyInterventions
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName != null && !propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public bool DirectInterventionBinding
    {
        get => IsValid && DirectIntervention;
        set =>
            this.Commit(() =>
            {
                DirectIntervention = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool UseOfIndividualsBinding
    {
        get => IsValid && UseOfIndividuals;
        set =>
            this.Commit(() =>
            {
                UseOfIndividuals = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool UseCommAgenciesBinding
    {
        get => IsValid && UseCommAgencies;
        set =>
            this.Commit(() =>
            {
                UseCommAgencies = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool ProtectVictimBinding
    {
        get => IsValid && ProtectVictim;
        set =>
            this.Commit(() =>
            {
                ProtectVictim = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool LeaveHomeBinding
    {
        get => IsValid && LeaveHome;
        set =>
            this.Commit(() =>
            {
                LeaveHome = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool NonOffendingParentBinding
    {
        get => IsValid && NonOffendingParent;
        set =>
            this.Commit(() =>
            {
                NonOffendingParent = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool LegalIntPlannedBinding
    {
        get => IsValid && LegalIntPlanned;
        set =>
            this.Commit(() =>
            {
                LegalIntPlanned = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool OtherSafetyInterventionsBinding
    {
        get => IsValid && OtherSafetyInterventions;
        set =>
            this.Commit(() =>
            {
                OtherSafetyInterventions = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public string CmtSafetyInterventionsBinding
    {
        get => IsValid ? CmtSafetyInterventions : string.Empty;
        set => this.Commit(() => CmtSafetyInterventions = value);
    }

    public bool ChildOutsideHomeBinding
    {
        get => IsValid && ChildOutsideHome;
        set =>
            this.Commit(() =>
            {
                ChildOutsideHome = value;
                if (value)
                    SetChildRemoved(false);
            });
    }

    public bool ChildRemovedBinding
    {
        get => IsValid && ChildRemoved;
        set => this.Commit(() => SetChildRemoved(value));
    }

    /// <summary>
    /// Used for business form logic: if ChildRemoved checked, all others unchecked. If any other checked after that,
    /// ChildRemoved is unchecked.
    /// </summary>
    /// <param name="newVal">Directly assigned to ChildRemoved</param>
    private void SetChildRemoved(bool newVal)
    {
        ChildRemoved = newVal;

        if (ChildRemoved)
        {
            DirectIntervention =
                UseOfIndividuals =
                UseCommAgencies =
                ProtectVictim =
                LeaveHome =
                NonOffendingParent =
                LegalIntPlanned =
                OtherSafetyInterventions =
                ChildOutsideHome =
                    false;
        }
    }
}
