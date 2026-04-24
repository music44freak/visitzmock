/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class SafetyAssessment
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName != null && !propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public string IncidentNumberBinding
    {
        get => IsValid ? IncidentNumber : string.Empty;
        set => this.Commit(() => IncidentNumber = value);
    }

    public string WorkerIdBinding
    {
        get => IsValid ? WorkerId : string.Empty;
        set => this.Commit(() => WorkerId = value);
    }

    public string FamilyNameBinding
    {
        get => IsValid ? FamilyName : string.Empty;
        set => this.Commit(() => FamilyName = value);
    }

    public DateTimeOffset? DateOfAssessmentBinding
    {
        get => IsValid ? DateOfAssessment : default;
        set => this.Commit(() => DateOfAssessment = value);
    }

    public DateTime? DateOfAssessmentBindingDateTimeWrapper
    {
        get => DateOfAssessmentBinding?.DateTime;
        set
        {
            if (value is DateTime dateTime)
                DateOfAssessmentBinding = new DateTimeOffset(dateTime);
            else
                DateOfAssessmentBinding = null;
        }
    }

    public string OperationBinding
    {
        get => IsValid ? Operation : string.Empty;
        set => this.Commit(() => Operation = value);
    }
}
