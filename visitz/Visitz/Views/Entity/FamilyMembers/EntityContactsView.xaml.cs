using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class EntityContactsView : IcmRecordContentView<EntityContactsViewModel>
{
    public EntityContactsView()
        : base(ServiceProvider.GetService<EntityContactsViewModel>(), LocalizedStrings.FamilyMembers)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
