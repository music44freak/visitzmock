using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Details;

public partial class EntityDetailsView : IcmRecordContentView<EntityDetailsViewModel>
{
    public EntityDetailsView()
        : base(ServiceProvider.GetService<EntityDetailsViewModel>(), LocalizedStrings.Details)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
