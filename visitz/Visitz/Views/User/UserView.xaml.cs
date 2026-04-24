using Visitz.Views.BaseClasses;

namespace Visitz.Views.User;

public partial class UserView : ViewModelContentView<UserViewModel>
{
    public UserView()
        : base(ServiceProvider.GetService<UserViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
