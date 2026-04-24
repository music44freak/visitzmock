using Visitz.Views.BaseClasses;

namespace Visitz.Views.Navigation;

public partial class HorizontalNavRailView : ViewModelContentView<NavRailViewModel>
{
    public HorizontalNavRailView()
        : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
