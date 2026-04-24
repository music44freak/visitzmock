using System.Windows.Input;

namespace Visitz.Views.Navigation;

public partial class NavDrawerItemView : ContentView
{
    public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create(
        nameof(TappedCommand),
        typeof(ICommand),
        typeof(NavDrawerItemView)
    );

    public ICommand TappedCommand
    {
        get => (ICommand)GetValue(TappedCommandProperty);
        set => SetValue(TappedCommandProperty, value);
    }

    public NavDrawerItemView()
    {
        InitializeComponent();
    }

    private void SfEffectsView_TouchUp(object? sender, EventArgs e)
    {
        TappedCommand?.Execute(null);
    }
}
