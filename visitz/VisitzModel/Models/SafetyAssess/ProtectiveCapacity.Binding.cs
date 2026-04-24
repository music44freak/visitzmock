/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class ProtectiveCapacity
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName != null && !propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public bool ChildCognitiveBinding
    {
        get => IsValid && ChildCognitive;
        set => this.Commit(() => ChildCognitive = value);
    }

    public bool ParentCognitiveBinding
    {
        get => IsValid && ParentCognitive;
        set
        {
            this.Commit(() => ParentCognitive = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentWillingnessBinding
    {
        get => IsValid && ParentWillingness;
        set
        {
            this.Commit(() => ParentWillingness = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentResourcesBinding
    {
        get => IsValid && ParentResources;
        set
        {
            this.Commit(() => ParentResources = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentSupportiveBinding
    {
        get => IsValid && ParentSupportive;
        set
        {
            this.Commit(() => ParentSupportive = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentProtectBinding
    {
        get => IsValid && ParentProtect;
        set
        {
            this.Commit(() => ParentProtect = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentAcceptBinding
    {
        get => IsValid && ParentAccept;
        set
        {
            this.Commit(() => ParentAccept = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentRelationshipBinding
    {
        get => IsValid && ParentRelationship;
        set
        {
            this.Commit(() => ParentRelationship = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentAwareBinding
    {
        get => IsValid && ParentAware;
        set
        {
            this.Commit(() => ParentAware = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool ParentProbSolvingBinding
    {
        get => IsValid && ParentProbSolving;
        set
        {
            this.Commit(() => ParentProbSolving = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public bool NoProCapPresentBinding
    {
        get => IsValid && NoProCapPresent;
        set => this.Commit(() => SetNoProCapPresent(value));
    }

    public bool CapacitiesOtherBinding
    {
        get => IsValid && CapacitiesOther;
        set
        {
            this.Commit(() => CapacitiesOther = value);
            if (value)
                ClearNoProCapPresent();
        }
    }

    public string CmtProtectiveCapacity01Binding
    {
        get => IsValid ? CmtProtectiveCapacity01 : string.Empty;
        set => this.Commit(() => CmtProtectiveCapacity01 = value);
    }

    public string CmtProtectiveCapacity02Binding
    {
        get => IsValid ? CmtProtectiveCapacity02 : string.Empty;
        set => this.Commit(() => CmtProtectiveCapacity02 = value);
    }
}
