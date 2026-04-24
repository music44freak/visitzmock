using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.SupportNetwork;

#nullable enable

public partial class SupportNetworkListView : IcmRecordContentView<SupportNetworkListViewModel>
{
    public SupportNetworkListView()
        : base(ServiceProvider.GetService<SupportNetworkListViewModel>(), LocalizedStrings.SupportNetwork)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
