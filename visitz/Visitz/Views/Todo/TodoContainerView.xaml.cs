using Visitz.Views.BaseClasses;

namespace Visitz.Views.Todo;

public partial class TodoContainerView : ViewModelContentView<TodoContainerViewModel>
{
    public TodoContainerView()
        : base(ServiceProvider.GetService<TodoContainerViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
