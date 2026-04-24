using Visitz.Views.BaseClasses;

namespace Visitz.Views.Todo;

#nullable enable

public partial class TodoListView : ViewModelContentView<TodoListViewModel>
{
    public TodoListView()
        : base(ServiceProvider.GetService<TodoListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
