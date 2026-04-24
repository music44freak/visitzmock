using VisitzModel;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class ChildInOutCareItem : ContentView
{
    public static readonly GridLength CheckBoxGridLength = new(50.0d);
    public static readonly GridLength LastNameGridLength = GridLength.Star;
    public static readonly GridLength FirstNameGridLength = GridLength.Star;
    public static readonly GridLength DateOfBirthGridLength = GridLength.Star;
    public static readonly GridLength GenderGridLength = GridLength.Star;

    public ChildInOutCareItem()
    {
        InitializeComponent();
        PropertyChanged += ChildInOutCareItem_PropertyChanged;
    }

    private void ChildInOutCareItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ConsoleTrace.TraceMethod(this, $"property: '{e.PropertyName}'");
    }
}
